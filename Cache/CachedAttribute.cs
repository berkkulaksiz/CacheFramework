// <copyright file="CachedAttribute.cs" project="Cache">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    berkkulaksiz
//    CreatedAt:   18.05.2025
//    UpdatedAt: 18.05.2025
// 
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Cache;

/// <summary>
///     Provides advanced caching functionality for controller actions with multiple strategies and policies.
/// </summary>
/// <remarks>
///     This attribute can be applied to controller methods to cache their responses with various caching strategies.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CachedAttribute : Attribute, IAsyncActionFilter
{
    // Circuit breaker for Redis connectivity issues
    private static readonly CircuitBreaker CircuitBreaker = new(
        3,
        TimeSpan.FromMinutes(1)
    );

    // Metrics
    private static readonly ICacheMetrics _metrics = new CacheMetrics();

    // AsyncLocal context for cross-cutting cache information
    private static readonly AsyncLocal<CacheContext> CacheContext = new();
    private readonly CachePolicy _cachePolicy;
    private readonly Type _cacheStrategyType;
    private readonly int _timeToLiveSeconds;
    private ICacheKeyGenerator _cacheKeyGenerator;
    private ICacheStrategy _cacheStrategy;
    private ILogger<CachedAttribute> _logger;
    private ICacheTimeoutProvider _timeoutProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CachedAttribute" /> class with basic settings.
    /// </summary>
    /// <param name="timeToLiveSeconds">Time to live in seconds for the cached item.</param>
    public CachedAttribute(int timeToLiveSeconds) : this(timeToLiveSeconds, CachePolicy.None,
        typeof(DefaultCacheStrategy))
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CachedAttribute" /> class with policy configuration.
    /// </summary>
    /// <param name="timeToLiveSeconds">Time to live in seconds for the cached item.</param>
    /// <param name="cachePolicy">The cache policy to apply.</param>
    public CachedAttribute(int timeToLiveSeconds, CachePolicy cachePolicy)
        : this(timeToLiveSeconds, cachePolicy, typeof(DefaultCacheStrategy))
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CachedAttribute" /> class with a custom strategy.
    /// </summary>
    /// <param name="timeToLiveSeconds">Time to live in seconds for the cached item.</param>
    /// <param name="cacheStrategyType">Type of the cache strategy to use.</param>
    public CachedAttribute(int timeToLiveSeconds, Type cacheStrategyType)
        : this(timeToLiveSeconds, CachePolicy.None, cacheStrategyType)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="CachedAttribute" /> class with full configuration.
    /// </summary>
    /// <param name="timeToLiveSeconds">Time to live in seconds for the cached item.</param>
    /// <param name="cachePolicy">The cache policy to apply.</param>
    /// <param name="cacheStrategyType">Type of the cache strategy to use.</param>
    [ActivatorUtilitiesConstructor]
    public CachedAttribute(int timeToLiveSeconds, CachePolicy cachePolicy, Type cacheStrategyType)
    {
        if (cacheStrategyType != null && !typeof(ICacheStrategy).IsAssignableFrom(cacheStrategyType))
            throw new ArgumentException($"The type {cacheStrategyType.Name} must implement ICacheStrategy.",
                nameof(cacheStrategyType));

        _timeToLiveSeconds = timeToLiveSeconds;
        _cachePolicy = cachePolicy;
        _cacheStrategyType = cacheStrategyType ?? typeof(DefaultCacheStrategy);
    }

    /// <summary>
    ///     Called asynchronously before the action, after model binding is complete.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The action execution delegate.</param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Initialize services
        InitializeServices(context);

        // Initialize cache context
        CacheContext.Value = new CacheContext
        {
            IsCacheable = true,
            RequestPath = context.HttpContext.Request.Path
        };

        // Get cache settings
        var cacheSettings = context.HttpContext.RequestServices.GetRequiredService<IRedisSettings>();

        // Skip caching if it's disabled
        if (!cacheSettings.Enabled)
        {
            _logger.LogDebug("Caching is disabled. Skipping cache operations.");
            await next().ConfigureAwait(false);
            return;
        }

        // Check if circuit breaker is open
        if (CircuitBreaker.IsOpen)
        {
            _logger.LogWarning("Cache circuit breaker is open. Using fallback caching approach.");
            await HandleWithCircuitBreakerOpen(context, next);
            return;
        }

        try
        {
            // Check if the request should be cached according to strategy
            if (!await _cacheStrategy.ShouldCacheResponse(context))
            {
                _logger.LogDebug("Request is not cacheable according to strategy. Skipping cache operations.");
                await next().ConfigureAwait(false);
                return;
            }

            // Get cache manager
            var cacheManager = context.HttpContext.RequestServices.GetRequiredService<ICacheManager<CacheEntry>>();

            // Generate cache key
            var cacheKey = await _cacheKeyGenerator.GenerateCacheKey(context.HttpContext.Request, _cachePolicy);
            CacheContext.Value.CacheKey = cacheKey;
            _logger.LogDebug("Generated cache key: {CacheKey}", cacheKey);

            // Handle GET requests - check cache before executing action
            if (context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
                using (_metrics.MeasureCacheLatency())
                {
                    var cachedEntryTask = cacheManager.GetAsync(cacheKey);
                    var executedContext = await next().ConfigureAwait(false);
                    var cachedEntry = await cachedEntryTask.ConfigureAwait(false);

                    // Check if we have a cached response
                    if (cachedEntry != null)
                    {
                        _metrics.IncrementCacheHits();
                        _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);

                        // Get the executed result
                        var executedContextResult = executedContext.Result as ObjectResult;
                        var executedContextValue = executedContextResult?.Value;

                        // Verify executed response is valid before comparison
                        if (executedContextValue == null) return;

                        var hashesAreEqual = false;
                        var etag = string.Empty;

                        try
                        {
                            // Compare the hashes to see if the cached response matches the current response
                            var executedContextContent = JsonConvert.SerializeObject(executedContextValue);
                            var hashOfExecutedContextContent = ComputeHash(executedContextContent);
                            var hashOfCachedResponse = cachedEntry.ETag;

                            hashesAreEqual = string.Equals(hashOfExecutedContextContent, hashOfCachedResponse);
                            etag = hashOfCachedResponse;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error comparing cached response with current response.");
                        }

                        // Add ETag header for HTTP cache validation
                        if (!string.IsNullOrEmpty(etag))
                            context.HttpContext.Response.Headers[HeaderNames.ETag] = $"\"{etag}\"";

                        // Check if client sent If-None-Match header and it matches our ETag
                        var ifNoneMatchHeader = context.HttpContext.Request.Headers[HeaderNames.IfNoneMatch].ToString();
                        if (!string.IsNullOrEmpty(ifNoneMatchHeader) && ifNoneMatchHeader.Contains(etag))
                        {
                            context.Result = new StatusCodeResult((int)HttpStatusCode.NotModified);
                            return;
                        }

                        switch (hashesAreEqual)
                        {
                            // If hashes match, return cached response (unless policy requires refresh)
                            case true when !_cachePolicy.HasFlag(CachePolicy.StaleWhileRevalidate):
                            {
                                _logger.LogDebug("Using cached response for key: {CacheKey}", cacheKey);

                                // Add cache control headers
                                context.HttpContext.Response.Headers[HeaderNames.CacheControl] =
                                    _cachePolicy.HasFlag(CachePolicy.CacheByUser)
                                        ? $"private, max-age={_timeToLiveSeconds}"
                                        : $"public, max-age={_timeToLiveSeconds}";

                                var contentResult = new ContentResult
                                {
                                    Content = cachedEntry.Content,
                                    ContentType = "application/json",
                                    StatusCode = (int)HttpStatusCode.OK
                                };

                                context.Result = contentResult;
                                return;
                            }
                            case false:
                                _logger.LogDebug("Cache hash mismatch. Updating cache for key: {CacheKey}", cacheKey);
                                break;
                            default:
                            {
                                if (_cachePolicy.HasFlag(CachePolicy.StaleWhileRevalidate))
                                {
                                    _logger.LogDebug(
                                        "Stale while revalidate policy active. Returning cached response but refreshing in background.");

                                    // Return the cached response immediately
                                    var contentResult = new ContentResult
                                    {
                                        Content = cachedEntry.Content,
                                        ContentType = "application/json",
                                        StatusCode = (int)HttpStatusCode.OK
                                    };

                                    context.Result = contentResult;

                                    // Refresh the cache in the background
                                    _ = Task.Run(async () =>
                                    {
                                        try
                                        {
                                            await UpdateCache(cacheManager, cacheKey, executedContextValue);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex,
                                                "Error updating cache in background for key: {CacheKey}", cacheKey);
                                        }
                                    });

                                    return;
                                }

                                break;
                            }
                        }

                        // Update cache with new result
                        await UpdateCache(cacheManager, cacheKey, executedContextValue);
                        return;
                    }

                    _metrics.IncrementCacheMisses();
                    _logger.LogDebug("Cache miss for key: {CacheKey}", cacheKey);

                    // Cache the response if it's a successful result (200 OK)
                    if (executedContext.Result is not ObjectResult objectResult ||
                        (objectResult.StatusCode != null && objectResult.StatusCode != (int)HttpStatusCode.OK))
                        return;

                    await UpdateCache(cacheManager, cacheKey, objectResult.Value);
                    return;
                }

            // For non-GET requests, process according to cache policy
            if (_cachePolicy.HasFlag(CachePolicy.InvalidateOnUpdate) &&
                !context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                await HandleNonGetRequest(context, next, cacheManager);
                return;
            }

            // If we reach here, just execute the action without any cache operations
            await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            CircuitBreaker.TrackException(ex);

            // Log the error but continue with the request
            _logger.LogError(ex, "Error in cache operation. Executing action without caching.");

            // Execute the action if it hasn't been executed yet
            if (context.Result == null) await next().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Initializes services required by the attribute.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    private void InitializeServices(ActionExecutingContext context)
    {
        var services = context.HttpContext.RequestServices;

        _logger = services.GetRequiredService<ILogger<CachedAttribute>>();

        _cacheKeyGenerator = services.GetService<ICacheKeyGenerator>() ??
                             new DefaultCacheKeyGenerator(services.GetService<ILogger<DefaultCacheKeyGenerator>>());

        _timeoutProvider = services.GetService<ICacheTimeoutProvider>() ??
                           new DefaultCacheTimeoutProvider();

        _cacheStrategy = services.GetService(_cacheStrategyType) as ICacheStrategy ??
                         Activator.CreateInstance(_cacheStrategyType) as ICacheStrategy ??
                         new DefaultCacheStrategy();
    }

    /// <summary>
    ///     Handles the request when the circuit breaker is open.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The action execution delegate.</param>
    private async Task HandleWithCircuitBreakerOpen(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            // Try to use memory cache as fallback
            var memoryCache = context.HttpContext.RequestServices.GetService<IMemoryCache>();

            if (memoryCache != null &&
                context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                var cacheKey = await _cacheKeyGenerator.GenerateCacheKey(context.HttpContext.Request, _cachePolicy);

                var cachedResponse = memoryCache.Get<CacheEntry>(cacheKey);

                if (cachedResponse != null)
                {
                    _logger.LogDebug("Serving from memory cache for key: {CacheKey}", cacheKey);

                    var contentResult = new ContentResult
                    {
                        Content = cachedResponse.Content,
                        ContentType = "application/json",
                        StatusCode = (int)HttpStatusCode.OK
                    };

                    context.Result = contentResult;
                    return;
                }

                // Execute action and cache the result in memory if successful
                var executedContext = await next().ConfigureAwait(false);

                if (executedContext.Result is ObjectResult objectResult &&
                    (objectResult.StatusCode == null || objectResult.StatusCode == (int)HttpStatusCode.OK))
                    try
                    {
                        var responseJson = JsonConvert.SerializeObject(objectResult.Value);
                        var responseHash = ComputeHash(responseJson);

                        var cacheEntry = new CacheEntry
                        {
                            Content = responseJson,
                            ETag = responseHash,
                            Timestamp = DateTimeOffset.UtcNow
                        };

                        memoryCache.Set(cacheKey, cacheEntry, TimeSpan.FromSeconds(_timeToLiveSeconds));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error caching response in memory cache for key: {CacheKey}", cacheKey);
                    }

                return;
            }

            // If memory cache is not available or not a GET request, execute without caching
            await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in fallback caching logic");
            await next().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Handles non-GET requests that might invalidate cache.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="next">The action execution delegate.</param>
    /// <param name="cacheManager">The cache manager.</param>
    private async Task HandleNonGetRequest(
        ActionExecutingContext context,
        ActionExecutionDelegate next,
        ICacheManager<CacheEntry> cacheManager)
    {
        // Execute the action
        var executedContext = await next().ConfigureAwait(false);

        // If successful, invalidate related cache entries
        if (executedContext.Result is not ObjectResult
            {
                StatusCode: (int)HttpStatusCode.OK or (int)HttpStatusCode.Created
                or (int)HttpStatusCode.NoContent
            }
           ) return;

        try
        {
            await _cacheStrategy.InvalidateRelatedCacheEntries(context, cacheManager);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating related cache entries");
        }
    }

    /// <summary>
    ///     Updates the cache with the specified value.
    /// </summary>
    /// <param name="cacheManager">The cache manager.</param>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    private async Task UpdateCache(ICacheManager<CacheEntry> cacheManager, string cacheKey, object value)
    {
        try
        {
            _logger.LogDebug("Updating cache for key: {CacheKey}", cacheKey);

            // Serialize the response
            var responseJson = JsonConvert.SerializeObject(value);
            var responseHash = ComputeHash(responseJson);

            // Determine actual timeout
            var timeout = _timeoutProvider.GetTimeout(cacheKey, _timeToLiveSeconds);

            // Create cache entry
            var cacheEntry = new CacheEntry
            {
                Content = responseJson,
                ETag = responseHash,
                Timestamp = DateTimeOffset.UtcNow
            };

            // Apply compression if configured
            if (_cachePolicy.HasFlag(CachePolicy.CompressContent) && responseJson.Length > 1024)
            {
                cacheEntry.Content = CompressContent(responseJson);
                cacheEntry.IsCompressed = true;
            }

            // Add tags from cache context
            if (CacheContext.Value?.Tags.Count > 0) cacheEntry.Tags = CacheContext.Value.Tags;

            await cacheManager.AddAsync(cacheKey, cacheEntry, timeout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cache for key: {CacheKey}", cacheKey);
        }
    }

    /// <summary>
    ///     Computes a hash for the specified content.
    /// </summary>
    /// <param name="content">The content to hash.</param>
    /// <returns>A string representation of the hash.</returns>
    private static string ComputeHash(string content)
    {
        using var sha = SHA256.Create();
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var hashBytes = sha.ComputeHash(contentBytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    ///     Compresses the specified content.
    /// </summary>
    /// <param name="content">The content to compress.</param>
    /// <returns>The compressed content.</returns>
    private static string CompressContent(string content)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);

        using var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
        {
            gzipStream.Write(contentBytes, 0, contentBytes.Length);
        }

        return Convert.ToBase64String(outputStream.ToArray());
    }
}
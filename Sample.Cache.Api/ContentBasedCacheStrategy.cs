// <copyright file="ContentBasedCacheStrategy.cs" project="Sample.Cache.Api">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    berkkulaksiz
//    CreatedAt:   18.05.2025
//    UpdatedAt: 18.05.2025
// 
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Sample.Cache.Api;

/// <summary>
///     Content-based caching strategy that caches responses based on content type.
/// </summary>
public class ContentBasedCacheStrategy : ICacheStrategy
{
    private readonly string[] _cachableContentTypes;
    private readonly ILogger<ContentBasedCacheStrategy> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContentBasedCacheStrategy" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ContentBasedCacheStrategy(ILogger<ContentBasedCacheStrategy> logger)
    {
        _logger = logger;
        _cachableContentTypes = new[] { "application/json" };
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContentBasedCacheStrategy" /> class
    ///     with specified content types.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="cachableContentTypes">The content types that should be cached.</param>
    public ContentBasedCacheStrategy(ILogger<ContentBasedCacheStrategy> logger, string[] cachableContentTypes)
    {
        _logger = logger;
        _cachableContentTypes = cachableContentTypes ?? new[] { "application/json" };
    }

    /// <inheritdoc />
    public Task<bool> ShouldCacheResponse(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(false);

        // Check Accept header to see if it matches cachable content types
        var acceptHeader = context.HttpContext.Request.Headers["Accept"].ToString();

        // If no Accept header, assume default content type is acceptable
        if (string.IsNullOrEmpty(acceptHeader)) return Task.FromResult(true);

        // Check if any of the cachable content types is accepted
        return Task.FromResult(_cachableContentTypes.Any(contentType => acceptHeader.Contains(contentType)));
    }

    /// <inheritdoc />
    public Task<bool> ShouldServeFromCache(object cachedResponse, object currentResponse)
    {
        // Default behavior - serve from cache if available
        return Task.FromResult(cachedResponse != null);
    }

    /// <inheritdoc />
    public async Task InvalidateRelatedCacheEntries(ActionExecutingContext context,
        ICacheManager<CacheEntry> cacheManager)
    {
        _logger.LogDebug("ContentBasedCacheStrategy: Invalidating related cache entries");

        // Invalidate based on controller/action path
        var path = context.HttpContext.Request.Path.ToString();
        var keysToInvalidate = cacheManager.GetRedisKeys(path + "*");

        if (keysToInvalidate.Any())
        {
            _logger.LogDebug("ContentBasedCacheStrategy: Invalidating {Count} cache entries for path: {Path}",
                keysToInvalidate.Count(), path);
            await cacheManager.DeleteAllAsync(keysToInvalidate.Select(k => k.ToString()));
        }
    }
}
// <copyright file="DefaultCacheKeyGenerator.cs" project="Cache">
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
///     Default implementation of <see cref="ICacheKeyGenerator" />.
/// </summary>
public class DefaultCacheKeyGenerator : ICacheKeyGenerator
{
    private readonly ILogger<DefaultCacheKeyGenerator> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultCacheKeyGenerator" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public DefaultCacheKeyGenerator(ILogger<DefaultCacheKeyGenerator> logger = null)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public virtual Task<string> GenerateCacheKey(HttpRequest request, CachePolicy cachePolicy)
    {
        var keyBuilder = new StringBuilder();

        // Add the path
        keyBuilder.Append($"{request.Path}");

        // Add query parameters if policy allows
        if (cachePolicy.HasFlag(CachePolicy.VaryByQueryParams))
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
                keyBuilder.Append($"|{key}-{value}");

        // Add HTTP method
        keyBuilder.Append($"|{request.Method}");

        // Add Accept-Encoding header if policy allows
        if (cachePolicy.HasFlag(CachePolicy.VaryByAcceptEncoding) &&
            request.Headers.TryGetValue("Accept-Encoding", out var acceptEncoding))
            keyBuilder.Append($"|ae-{acceptEncoding}");

        // Add user identifier if policy requires user-specific caching
        if (cachePolicy.HasFlag(CachePolicy.CacheByUser) &&
            request.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId)) keyBuilder.Append($"|user-{userId}");
        }

        // Apply hashing for long keys to prevent Redis performance issues
        var cacheKey = keyBuilder.ToString();
        if (cacheKey.Length > 100)
        {
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(cacheKey));
            var hash = Convert.ToBase64String(hashBytes);

            // Keep the path for readability but hash the rest
            var path = request.Path.ToString();
            cacheKey = $"{path}|h-{hash}";

            _logger?.LogDebug("Generated hashed cache key for long key, original length: {OriginalLength}",
                keyBuilder.Length);
        }

        return Task.FromResult(cacheKey);
    }
}
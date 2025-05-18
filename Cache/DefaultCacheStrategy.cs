// <copyright file="DefaultCacheStrategy.cs" project="Cache">
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
///     Default implementation of <see cref="ICacheStrategy" />.
/// </summary>
public class DefaultCacheStrategy : ICacheStrategy
{
    /// <inheritdoc />
    public virtual Task<bool> ShouldCacheResponse(ActionExecutingContext context)
    {
        // By default, only cache GET requests
        return Task.FromResult(context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public Task<bool> ShouldServeFromCache(object cachedResponse, object currentResponse)
    {
        // By default, always serve from cache if available
        return Task.FromResult(cachedResponse != null);
    }

    /// <inheritdoc />
    public virtual async Task InvalidateRelatedCacheEntries(ActionExecutingContext context,
        ICacheManager<CacheEntry> cacheManager)
    {
        // Get all keys matching the path pattern
        var pathPattern = context.HttpContext.Request.Path.ToString();
        var keysToInvalidate = cacheManager.GetRedisKeys(pathPattern + "*");

        if (keysToInvalidate.Any())
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<DefaultCacheStrategy>>();
            logger?.LogDebug("Invalidating {Count} cache entries for path: {Path}", keysToInvalidate.Count(),
                pathPattern);
            await cacheManager.DeleteAllAsync(keysToInvalidate.Select(k => k.ToString()));
        }
    }
}
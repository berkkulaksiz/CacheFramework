// <copyright file="ICacheStrategy.cs" project="Cache">
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
///     Defines a strategy for caching responses.
/// </summary>
public interface ICacheStrategy
{
    /// <summary>
    ///     Determines whether a response should be cached.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <returns>True if the response should be cached; otherwise, false.</returns>
    Task<bool> ShouldCacheResponse(ActionExecutingContext context);

    /// <summary>
    ///     Determines whether a response should be served from cache.
    /// </summary>
    /// <param name="cachedResponse">The cached response.</param>
    /// <param name="currentResponse">The current response.</param>
    /// <returns>True if the response should be served from cache; otherwise, false.</returns>
    Task<bool> ShouldServeFromCache(object cachedResponse, object currentResponse);

    /// <summary>
    ///     Invalidates related cache entries after a non-GET request.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    /// <param name="cacheManager">The cache manager.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvalidateRelatedCacheEntries(ActionExecutingContext context, ICacheManager<CacheEntry> cacheManager);
}
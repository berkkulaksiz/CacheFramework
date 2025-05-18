// <copyright file="ICacheTimeoutProvider.cs" project="Cache">
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
///     Interface for providing cache timeouts.
/// </summary>
public interface ICacheTimeoutProvider
{
    /// <summary>
    ///     Gets the timeout for a cache entry.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="defaultTimeToLiveSeconds">The default time to live in seconds.</param>
    /// <returns>The timeout for the cache entry.</returns>
    TimeSpan GetTimeout(string cacheKey, int defaultTimeToLiveSeconds);
}
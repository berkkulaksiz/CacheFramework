// <copyright file="ICacheKeyGenerator.cs" project="Cache">
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
///     Interface for generating cache keys from HTTP requests.
/// </summary>
public interface ICacheKeyGenerator
{
    /// <summary>
    ///     Generates a cache key from an HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="cachePolicy">The cache policy to apply.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cache key.</returns>
    Task<string> GenerateCacheKey(HttpRequest request, CachePolicy cachePolicy);
}
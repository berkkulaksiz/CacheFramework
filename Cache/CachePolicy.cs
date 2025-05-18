// <copyright file="CachePolicy.cs" project="Cache">
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
///     Defines cache policies that can be applied to cached responses.
/// </summary>
[Flags]
public enum CachePolicy
{
    /// <summary>
    ///     No specific cache policy.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Cache responses per user (requires authentication).
    /// </summary>
    CacheByUser = 1,

    /// <summary>
    ///     Return stale cache while revalidating in the background.
    /// </summary>
    StaleWhileRevalidate = 2,

    /// <summary>
    ///     Invalidate related cache entries on update operations.
    /// </summary>
    InvalidateOnUpdate = 4,

    /// <summary>
    ///     Vary cache by Accept-Encoding header.
    /// </summary>
    VaryByAcceptEncoding = 8,

    /// <summary>
    ///     Compress cached content.
    /// </summary>
    CompressContent = 16,

    /// <summary>
    ///     Allow caching of responses with varying query parameters.
    /// </summary>
    VaryByQueryParams = 32,

    /// <summary>
    ///     Cache responses even for authenticated users.
    /// </summary>
    CacheAuthenticated = 64,

    /// <summary>
    ///     Set reasonable defaults for most scenarios.
    /// </summary>
    DefaultPolicy = InvalidateOnUpdate,

    /// <summary>
    ///     Set a policy optimized for API responses.
    /// </summary>
    ApiPolicy = InvalidateOnUpdate | CompressContent | VaryByQueryParams,

    /// <summary>
    ///     Set a policy optimized for authenticated users.
    /// </summary>
    AuthenticatedUserPolicy = CacheByUser | InvalidateOnUpdate | CacheAuthenticated,

    /// <summary>
    ///     Set a policy optimized for high-performance scenarios.
    /// </summary>
    HighPerformancePolicy = StaleWhileRevalidate | CompressContent | InvalidateOnUpdate
}
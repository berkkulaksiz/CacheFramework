// <copyright file="CacheDescriptorProvider.cs" project="Cache">
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
///     Provides cache descriptors for OpenAPI documentation.
/// </summary>
public class CacheDescriptorProvider
{
    private readonly CachingOptions _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CacheDescriptorProvider" /> class.
    /// </summary>
    /// <param name="options">The caching options.</param>
    public CacheDescriptorProvider(CachingOptions options)
    {
        _options = options;
    }

    /// <summary>
    ///     Gets the cache descriptor for an endpoint.
    /// </summary>
    /// <param name="cacheDuration">The cache duration.</param>
    /// <param name="cachePolicy">The cache policy.</param>
    /// <returns>The cache descriptor.</returns>
    public CacheDescriptor GetCacheDescriptor(int cacheDuration, string cachePolicy)
    {
        return new CacheDescriptor
        {
            Duration = cacheDuration > 0 ? cacheDuration : _options.DefaultTimeToLiveSeconds,
            Policy = cachePolicy
        };
    }
}
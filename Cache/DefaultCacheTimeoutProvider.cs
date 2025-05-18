// <copyright file="DefaultCacheTimeoutProvider.cs" project="Cache">
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
///     Default implementation of <see cref="ICacheTimeoutProvider" />.
/// </summary>
public class DefaultCacheTimeoutProvider : ICacheTimeoutProvider
{
    /// <inheritdoc />
    public TimeSpan GetTimeout(string cacheKey, int defaultTimeToLiveSeconds)
    {
        return TimeSpan.FromSeconds(defaultTimeToLiveSeconds);
    }
}
// <copyright file="CacheContext.cs" project="Cache">
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
///     Context for caching operations that can be passed through the execution context.
/// </summary>
public class CacheContext
{
    /// <summary>
    ///     Gets or sets a value indicating whether the response is cacheable.
    /// </summary>
    public bool IsCacheable { get; set; } = true;

    /// <summary>
    ///     Gets or sets the cache key.
    /// </summary>
    public string CacheKey { get; set; }

    /// <summary>
    ///     Gets or sets the request path.
    /// </summary>
    public string RequestPath { get; set; }

    /// <summary>
    ///     Gets the tags associated with the cache entry.
    /// </summary>
    public Dictionary<string, string> Tags { get; } = new();

    /// <summary>
    ///     Adds a tag to the cache entry.
    /// </summary>
    /// <param name="key">The tag key.</param>
    /// <param name="value">The tag value.</param>
    public void AddTag(string key, string value)
    {
        if (!string.IsNullOrEmpty(key)) Tags[key] = value;
    }
}
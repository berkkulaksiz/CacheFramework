// <copyright file="CacheDescriptor.cs" project="Cache">
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
///     Descriptor for cache settings used in OpenAPI documentation.
/// </summary>
public class CacheDescriptor
{
    /// <summary>
    ///     Gets or sets the cache duration in seconds.
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    ///     Gets or sets the cache policy description.
    /// </summary>
    public string Policy { get; set; }
}
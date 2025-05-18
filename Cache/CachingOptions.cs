// <copyright file="CachingOptions.cs" project="Cache">
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
///     Options for configuring caching services.
/// </summary>
public class CachingOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Gets or sets the default time to live in seconds.
    /// </summary>
    public int DefaultTimeToLiveSeconds { get; set; } = 60;

    /// <summary>
    ///     Gets or sets the size limit in bytes.
    /// </summary>
    public long SizeLimitBytes { get; set; } = 1024 * 1024 * 10; // 10 MB

    /// <summary>
    ///     Gets or sets the minimum size in bytes for compression.
    /// </summary>
    public int CompressionThresholdBytes { get; set; } = 1024; // 1 KB

    /// <summary>
    ///     Gets or sets a value indicating whether to enable background refresh.
    /// </summary>
    public bool EnableBackgroundRefresh { get; set; } = false;

    /// <summary>
    ///     Gets or sets a value indicating whether to enable Swagger documentation.
    /// </summary>
    public bool EnableSwaggerDocumentation { get; set; } = true;
}
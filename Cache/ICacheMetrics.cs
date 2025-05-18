// <copyright file="ICacheMetrics.cs" project="Cache">
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
///     Interface for cache metrics.
/// </summary>
public interface ICacheMetrics
{
    /// <summary>
    ///     Increments the cache hits counter.
    /// </summary>
    void IncrementCacheHits();

    /// <summary>
    ///     Increments the cache misses counter.
    /// </summary>
    void IncrementCacheMisses();

    /// <summary>
    ///     Measures cache operation latency.
    /// </summary>
    /// <returns>A disposable that stops the timer when disposed.</returns>
    IDisposable MeasureCacheLatency();

    /// <summary>
    ///     Gets the cache hit rate.
    /// </summary>
    /// <returns>The cache hit rate as a percentage.</returns>
    double GetHitRate();

    /// <summary>
    ///     Gets the average cache latency.
    /// </summary>
    /// <returns>The average cache latency in milliseconds.</returns>
    double GetAverageLatency();

    /// <summary>
    ///     Resets the metrics.
    /// </summary>
    void Reset();
}
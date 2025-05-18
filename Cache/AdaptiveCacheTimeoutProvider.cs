// <copyright file="AdaptiveCacheTimeoutProvider.cs" project="Cache">
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
///     Adaptive cache timeout provider that adjusts timeouts based on usage patterns.
/// </summary>
public class AdaptiveCacheTimeoutProvider : ICacheTimeoutProvider
{
    private readonly double _hitMultiplier;
    private readonly TimeSpan _maxTimeout;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _minTimeout;
    private readonly double _missMultiplier;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AdaptiveCacheTimeoutProvider" /> class.
    /// </summary>
    /// <param name="memoryCache">The memory cache.</param>
    /// <param name="minTimeoutSeconds">The minimum timeout in seconds.</param>
    /// <param name="maxTimeoutSeconds">The maximum timeout in seconds.</param>
    /// <param name="hitMultiplier">The multiplier for cache hits.</param>
    /// <param name="missMultiplier">The multiplier for cache misses.</param>
    public AdaptiveCacheTimeoutProvider(
        IMemoryCache memoryCache,
        int minTimeoutSeconds = 10,
        int maxTimeoutSeconds = 3600,
        double hitMultiplier = 1.5,
        double missMultiplier = 0.5)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _minTimeout = TimeSpan.FromSeconds(minTimeoutSeconds);
        _maxTimeout = TimeSpan.FromSeconds(maxTimeoutSeconds);
        _hitMultiplier = hitMultiplier;
        _missMultiplier = missMultiplier;
    }

    /// <inheritdoc />
    public TimeSpan GetTimeout(string cacheKey, int defaultTimeToLiveSeconds)
    {
        var statsKey = $"cache_stats:{cacheKey}";

        var stats = _memoryCache.GetOrCreate(statsKey, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromDays(1);
            return new UsageStats
            {
                Hits = 0,
                Misses = 0,
                CurrentTimeout = TimeSpan.FromSeconds(defaultTimeToLiveSeconds),
                LastUpdated = DateTimeOffset.UtcNow
            };
        });

        // Track cache hit
        stats.Hits++;
        stats.LastUpdated = DateTimeOffset.UtcNow;

        // Adjust timeout based on hit/miss ratio
        if (stats.Hits + stats.Misses >= 10)
        {
            var hitRatio = (double)stats.Hits / (stats.Hits + stats.Misses);

            if (hitRatio > 0.8)
            {
                // High hit ratio, increase timeout
                stats.CurrentTimeout = TimeSpan.FromTicks((long)(stats.CurrentTimeout.Ticks * _hitMultiplier));

                // Cap at max timeout
                if (stats.CurrentTimeout > _maxTimeout) stats.CurrentTimeout = _maxTimeout;
            }
            else if (hitRatio < 0.2)
            {
                // Low hit ratio, decrease timeout
                stats.CurrentTimeout = TimeSpan.FromTicks((long)(stats.CurrentTimeout.Ticks * _missMultiplier));

                // Cap at min timeout
                if (stats.CurrentTimeout < _minTimeout) stats.CurrentTimeout = _minTimeout;
            }

            // Reset counters after adjustment
            stats.Hits = 0;
            stats.Misses = 0;
        }

        return stats.CurrentTimeout;
    }

    /// <summary>
    ///     Tracks a cache miss for the specified key.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    public void TrackMiss(string cacheKey)
    {
        var statsKey = $"cache_stats:{cacheKey}";

        var stats = _memoryCache.GetOrCreate(statsKey, entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromDays(1);
            return new UsageStats
            {
                Hits = 0,
                Misses = 0,
                CurrentTimeout = TimeSpan.FromSeconds(60),
                LastUpdated = DateTimeOffset.UtcNow
            };
        });

        stats.Misses++;
        stats.LastUpdated = DateTimeOffset.UtcNow;
    }

    private class UsageStats
    {
        public int Hits { get; set; }
        public int Misses { get; set; }
        public TimeSpan CurrentTimeout { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
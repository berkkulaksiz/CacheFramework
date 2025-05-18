// <copyright file="CacheMetrics.cs" project="Cache">
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
///     Provides metrics for cache operations.
/// </summary>
public class CacheMetrics : ICacheMetrics
{
    private const int MaxLatencySamples = 1000;
    private readonly ConcurrentQueue<long> _latencies = new();
    private long _cacheHits;
    private long _cacheMisses;

    /// <inheritdoc />
    public void IncrementCacheHits()
    {
        Interlocked.Increment(ref _cacheHits);
    }

    /// <inheritdoc />
    public void IncrementCacheMisses()
    {
        Interlocked.Increment(ref _cacheMisses);
    }

    /// <inheritdoc />
    public IDisposable MeasureCacheLatency()
    {
        return new LatencyMeasurement(this);
    }

    /// <inheritdoc />
    public double GetHitRate()
    {
        var hits = Interlocked.Read(ref _cacheHits);
        var misses = Interlocked.Read(ref _cacheMisses);
        var total = hits + misses;

        return total == 0 ? 0 : (double)hits / total * 100;
    }

    /// <inheritdoc />
    public double GetAverageLatency()
    {
        if (_latencies.IsEmpty) return 0;

        return _latencies.Average() / 10000.0; // Convert ticks to milliseconds
    }

    /// <inheritdoc />
    public void Reset()
    {
        Interlocked.Exchange(ref _cacheHits, 0);
        Interlocked.Exchange(ref _cacheMisses, 0);

        while (_latencies.TryDequeue(out _))
        {
            // Empty the queue
        }
    }

    private void RecordLatency(long ticks)
    {
        _latencies.Enqueue(ticks);

        // Trim the queue if it gets too large
        while (_latencies.Count > MaxLatencySamples && _latencies.TryDequeue(out _))
        {
            // Remove oldest entries
        }
    }

    private class LatencyMeasurement : IDisposable
    {
        private readonly CacheMetrics _metrics;
        private readonly Stopwatch _stopwatch;

        public LatencyMeasurement(CacheMetrics metrics)
        {
            _metrics = metrics;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _metrics.RecordLatency(_stopwatch.ElapsedTicks);
        }
    }
}
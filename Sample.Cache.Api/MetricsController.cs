// <copyright file="MetricsController.cs" project="Sample.Cache.Api">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    berkkulaksiz
//    CreatedAt:   18.05.2025
//    UpdatedAt: 18.05.2025
// 
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

namespace Sample.Cache.Api;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly ICacheMetrics _cacheMetrics;
    private readonly ILogger<MetricsController> _logger;
    private readonly IMemoryCache _memoryCache;

    public MetricsController(
        ICacheMetrics cacheMetrics,
        IMemoryCache memoryCache,
        ILogger<MetricsController> logger)
    {
        _cacheMetrics = cacheMetrics;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    /// <summary>
    ///     Gets cache metrics and statistics
    /// </summary>
    [HttpGet]
    public IActionResult GetMetrics()
    {
        var hitRate = _cacheMetrics.GetHitRate();
        var avgLatency = _cacheMetrics.GetAverageLatency();

        _logger.LogInformation("Cache metrics - Hit Rate: {HitRate}%, Average Latency: {AvgLatency}ms",
            hitRate.ToString("F2"), avgLatency.ToString("F2"));

        return Ok(new
        {
            HitRate = hitRate,
            AverageLatency = avgLatency,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    ///     Resets cache metrics
    /// </summary>
    [HttpPost("reset")]
    public IActionResult ResetMetrics()
    {
        _logger.LogInformation("Resetting cache metrics");
        _cacheMetrics.Reset();
        return Ok(new { Message = "Cache metrics reset successfully" });
    }

    /// <summary>
    ///     Clears all cached items
    /// </summary>
    [HttpPost("clear")]
    public IActionResult ClearCache()
    {
        _logger.LogWarning("Clearing all cached items");

        // For the purpose of this demo, we're using a simple approach
        // In a real application with Redis, you'd use cacheManager.FlushAllDatabases()
        if (_memoryCache is MemoryCache memoryCache) memoryCache.Compact(1.0);

        return Ok(new { Message = "Cache cleared successfully" });
    }
}
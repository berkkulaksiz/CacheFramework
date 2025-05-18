// <copyright file="PathBasedCacheTimeoutProvider.cs" project="Cache">
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
///     Path-based cache timeout provider that provides different timeouts based on request path.
/// </summary>
public class PathBasedCacheTimeoutProvider : ICacheTimeoutProvider
{
    private readonly TimeSpan _defaultTimeout;
    private readonly Dictionary<string, TimeSpan> _pathTimeouts;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PathBasedCacheTimeoutProvider" /> class.
    /// </summary>
    /// <param name="pathTimeouts">The path timeouts.</param>
    /// <param name="defaultTimeoutSeconds">The default timeout in seconds.</param>
    public PathBasedCacheTimeoutProvider(Dictionary<string, TimeSpan> pathTimeouts, int defaultTimeoutSeconds = 60)
    {
        _pathTimeouts = pathTimeouts ?? new Dictionary<string, TimeSpan>();
        _defaultTimeout = TimeSpan.FromSeconds(defaultTimeoutSeconds);
    }

    /// <inheritdoc />
    public TimeSpan GetTimeout(string cacheKey, int defaultTimeToLiveSeconds)
    {
        // Extract path from cache key (assumes path is the first part before any '|')
        var path = cacheKey.Contains('|') ? cacheKey.Substring(0, cacheKey.IndexOf('|')) : cacheKey;

        // Look for exact match
        if (_pathTimeouts.TryGetValue(path, out var timeout)) return timeout;

        // Look for prefix match
        foreach (var (pathPrefix, pathTimeout) in _pathTimeouts)
            if (path.StartsWith(pathPrefix))
                return pathTimeout;

        // Fall back to provided default or class default
        return defaultTimeToLiveSeconds > 0
            ? TimeSpan.FromSeconds(defaultTimeToLiveSeconds)
            : _defaultTimeout;
    }
}
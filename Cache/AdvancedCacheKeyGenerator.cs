// <copyright file="AdvancedCacheKeyGenerator.cs" project="Cache">
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
///     Advanced cache key generator that supports custom key prefixes and suffixes.
/// </summary>
public class AdvancedCacheKeyGenerator : DefaultCacheKeyGenerator
{
    private readonly Func<HttpRequest, Task<string>> _customKeyPartGenerator;
    private readonly string _keyPrefix;
    private readonly string _keySuffix;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AdvancedCacheKeyGenerator" /> class.
    /// </summary>
    /// <param name="keyPrefix">The key prefix.</param>
    /// <param name="keySuffix">The key suffix.</param>
    /// <param name="customKeyPartGenerator">The custom key part generator.</param>
    /// <param name="logger">The logger.</param>
    public AdvancedCacheKeyGenerator(
        string keyPrefix = null,
        string keySuffix = null,
        Func<HttpRequest, Task<string>> customKeyPartGenerator = null,
        ILogger<DefaultCacheKeyGenerator> logger = null)
        : base(logger)
    {
        _keyPrefix = keyPrefix;
        _keySuffix = keySuffix;
        _customKeyPartGenerator = customKeyPartGenerator;
    }

    /// <inheritdoc />
    public override async Task<string> GenerateCacheKey(HttpRequest request, CachePolicy cachePolicy)
    {
        var baseKey = await base.GenerateCacheKey(request, cachePolicy);

        var keyBuilder = new StringBuilder();

        // Add prefix if any
        if (!string.IsNullOrEmpty(_keyPrefix)) keyBuilder.Append(_keyPrefix).Append(':');

        // Add base key
        keyBuilder.Append(baseKey);

        // Add custom key part if generator is provided
        if (_customKeyPartGenerator != null)
        {
            var customPart = await _customKeyPartGenerator(request);
            if (!string.IsNullOrEmpty(customPart)) keyBuilder.Append("|custom-").Append(customPart);
        }

        // Add suffix if any
        if (!string.IsNullOrEmpty(_keySuffix)) keyBuilder.Append(':').Append(_keySuffix);

        return keyBuilder.ToString();
    }
}
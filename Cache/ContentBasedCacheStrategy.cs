// <copyright file="ContentBasedCacheStrategyOptions.cs" project="Cache">
// 
//    Copyright (c) MicroFrame Solutions. All rights reserved.
//    Author:    berkkulaksiz
//    CreatedAt:   18.05.2025
//    UpdatedAt: 18.05.2025
// 
//    Licensed under the Proprietary license. See LICENSE file in the project root for full license information.
// 
// </copyright>

using Microsoft.Extensions.Options;

namespace Cache;

/// <summary>
/// Options for content-based cache strategy.
/// </summary>
public class ContentBasedCacheStrategyOptions
{
    /// <summary>
    /// Gets or sets the cachable content types.
    /// </summary>
    public string[] CachableContentTypes { get; set; } = ["application/json"];
}

/// <summary>
///     Content-based caching strategy that caches responses based on content type.
/// </summary>
public class ContentBasedCacheStrategy : DefaultCacheStrategy
{
    private readonly string[] _cachableContentTypes;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContentBasedCacheStrategy" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public ContentBasedCacheStrategy(IOptions<ContentBasedCacheStrategyOptions> options)
    {
        _cachableContentTypes = options.Value.CachableContentTypes?.Length > 0 
            ? options.Value.CachableContentTypes 
            : ["application/json"];
    }

    /// <inheritdoc />
    public override Task<bool> ShouldCacheResponse(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(false);

        // Check Accept header to see if it matches cachable content types
        var acceptHeader = context.HttpContext.Request.Headers["Accept"].ToString();

        // If no Accept header, assume default content type is acceptable
        if (string.IsNullOrEmpty(acceptHeader)) return Task.FromResult(true);

        // Check if any of the cachable content types is accepted
        return Task.FromResult(_cachableContentTypes.Any(contentType => acceptHeader.Contains(contentType)));
    }
}
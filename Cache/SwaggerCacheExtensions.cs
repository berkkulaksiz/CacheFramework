// <copyright file="SwaggerCacheExtensions.cs" project="Cache">
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
///     Provides extension methods for Swagger to document caching behavior.
/// </summary>
public static class SwaggerCacheExtensions
{
    /// <summary>
    ///     Adds cache documentation to Swagger.
    /// </summary>
    /// <param name="options">The Swagger generation options.</param>
    /// <returns>The Swagger generation options.</returns>
    public static SwaggerGenOptions AddCacheDocumentation(this SwaggerGenOptions options)
    {
        options.OperationFilter<CacheOperationFilter>();
        options.DocumentFilter<CacheDocumentFilter>();

        return options;
    }
}
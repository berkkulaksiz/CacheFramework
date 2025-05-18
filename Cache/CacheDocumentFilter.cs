// <copyright file="CacheDocumentFilter.cs" project="Cache">
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
///     Document filter for adding cache information to Swagger.
/// </summary>
public class CacheDocumentFilter : IDocumentFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Add cache information to the document
        if (swaggerDoc.Info.Description == null) swaggerDoc.Info.Description = "";

        swaggerDoc.Info.Description += "\n\n## Caching\n\nThis API uses caching to improve performance. " +
                                       "Endpoints with cache support are documented with cache duration and policy information. " +
                                       "Cache validation is supported through ETag headers.";

        // Add cache-related schemas
        swaggerDoc.Components.Schemas.Add("CachePolicy", new OpenApiSchema
        {
            Type = "string",
            Enum = Enum.GetNames(typeof(CachePolicy)).Select(n => new OpenApiString(n)).Cast<IOpenApiAny>().ToList(),
            Description = "Cache policy options for API responses"
        });
    }
}
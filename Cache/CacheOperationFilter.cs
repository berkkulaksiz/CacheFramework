// <copyright file="CacheOperationFilter.cs" project="Cache">
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
///     Operation filter for documenting caching behavior in Swagger.
/// </summary>
public class CacheOperationFilter : IOperationFilter
{
    private readonly CacheDescriptorProvider _descriptorProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CacheOperationFilter" /> class.
    /// </summary>
    /// <param name="descriptorProvider">The descriptor provider.</param>
    public CacheOperationFilter(CacheDescriptorProvider descriptorProvider)
    {
        _descriptorProvider = descriptorProvider;
    }

    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Find CachedAttribute on the controller action
        var cachedAttribute = context.MethodInfo.GetCustomAttributes(true)
            .Union(context.MethodInfo.DeclaringType.GetCustomAttributes(true))
            .OfType<CachedAttribute>()
            .FirstOrDefault();

        if (cachedAttribute == null) return;

        // Get cache duration from the attribute
        var cacheDuration = cachedAttribute.GetType().GetField("_timeToLiveSeconds",
            BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(cachedAttribute) as int? ?? 0;

        // Get cache policy from the attribute
        var cachePolicy = cachedAttribute.GetType().GetField("_cachePolicy",
                                  BindingFlags.Instance | BindingFlags.NonPublic)
                              ?.GetValue(cachedAttribute) as CachePolicy? ??
                          CachePolicy.None;

        // Add cache description to the operation
        var descriptor = _descriptorProvider.GetCacheDescriptor(cacheDuration, cachePolicy.ToString());

        if (operation.Description == null) operation.Description = "";

        operation.Description +=
            $"\n\n**Cache Information**\n- Duration: {descriptor.Duration} seconds\n- Policy: {descriptor.Policy}";

        // Add 304 response for ETag caching
        operation.Responses.Add("304", new OpenApiResponse
        {
            Description = "Not Modified (Content is unchanged and served from cache)"
        });

        // Add ETag header parameter
        if (!operation.Parameters.Any(p => p.Name == "If-None-Match"))
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "If-None-Match",
                In = ParameterLocation.Header,
                Description = "ETag value for cache validation",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
    }
}
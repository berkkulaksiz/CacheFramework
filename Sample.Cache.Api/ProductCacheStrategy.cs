// <copyright file="ProductCacheStrategy.cs" project="Sample.Cache.Api">
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

/// <summary>
///     Custom cache strategy for product-related endpoints
/// </summary>
public class ProductCacheStrategy : ICacheStrategy
{
    private readonly ILogger<ProductCacheStrategy> _logger;

    public ProductCacheStrategy(ILogger<ProductCacheStrategy> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<bool> ShouldCacheResponse(ActionExecutingContext context)
    {
        // Only cache GET requests
        if (!context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(false);

        _logger.LogDebug("ProductCacheStrategy: Evaluating if response should be cached");

        // Check if the request is for a specific product by ID or category
        var routeData = context.RouteData.Values;
        var hasIdOrCategory = routeData.ContainsKey("id") || routeData.ContainsKey("categoryId");

        // Check if the request is for featured products
        var path = context.HttpContext.Request.Path.ToString().ToLowerInvariant();
        var isFeaturedRequest = path.Contains("featured");

        _logger.LogDebug(
            "ProductCacheStrategy: HasIdOrCategory={HasIdOrCategory}, IsFeaturedRequest={IsFeaturedRequest}",
            hasIdOrCategory, isFeaturedRequest);

        // Cache specific product requests and featured product requests
        // Don't cache the general product list (which could be large)
        return Task.FromResult(hasIdOrCategory || isFeaturedRequest);
    }

    /// <inheritdoc />
    public Task<bool> ShouldServeFromCache(object cachedResponse, object currentResponse)
    {
        // Default behavior - serve from cache if available
        return Task.FromResult(cachedResponse != null);
    }

    /// <inheritdoc />
    public async Task InvalidateRelatedCacheEntries(ActionExecutingContext context,
        ICacheManager<CacheEntry> cacheManager)
    {
        _logger.LogDebug("ProductCacheStrategy: Invalidating related cache entries");

        // Get the product ID from the route data
        if (context.RouteData.Values.TryGetValue("id", out var idObj) &&
            int.TryParse(idObj?.ToString(), out var productId))
        {
            // 1. Invalidate specific product cache
            var productKey = $"/api/products/{productId}|*";
            var productKeyToInvalidate = cacheManager.GetRedisKeys(productKey);

            if (productKeyToInvalidate.Any())
            {
                _logger.LogDebug("ProductCacheStrategy: Invalidating cache for product ID {ProductId}", productId);
                await cacheManager.DeleteAllAsync(productKeyToInvalidate.Select(k => k.ToString()));
            }

            // 2. Try to get the category ID to invalidate category-specific product caches
            // In a real app, you would get this from the updated product data
            var categoryId = 0; // Default value

            // For the purpose of this demo, extract from action arguments
            if (context.ActionArguments.TryGetValue("product", out var productObj) &&
                productObj is Product product)
                categoryId = product.CategoryId;

            if (categoryId > 0)
            {
                var categoryProductsKey = $"/api/products/category/{categoryId}|*";
                var categoryProductsKeysToInvalidate = cacheManager.GetRedisKeys(categoryProductsKey);

                if (categoryProductsKeysToInvalidate.Any())
                {
                    _logger.LogDebug("ProductCacheStrategy: Invalidating cache for products in category {CategoryId}",
                        categoryId);
                    await cacheManager.DeleteAllAsync(categoryProductsKeysToInvalidate.Select(k => k.ToString()));
                }
            }

            // 3. Always invalidate featured products cache if a product is updated
            var featuredKey = "/api/products/featured|*";
            var featuredKeysToInvalidate = cacheManager.GetRedisKeys(featuredKey);

            if (featuredKeysToInvalidate.Any())
            {
                _logger.LogDebug("ProductCacheStrategy: Invalidating featured products cache");
                await cacheManager.DeleteAllAsync(featuredKeysToInvalidate.Select(k => k.ToString()));
            }
        }
        else
        {
            // If no specific ID is found, invalidate all product caches
            var allProductsKey = "/api/products*";
            var allKeysToInvalidate = cacheManager.GetRedisKeys(allProductsKey);

            if (allKeysToInvalidate.Any())
            {
                _logger.LogDebug("ProductCacheStrategy: Invalidating all product caches ({Count} items)",
                    allKeysToInvalidate.Count());
                await cacheManager.DeleteAllAsync(allKeysToInvalidate.Select(k => k.ToString()));
            }
        }
    }
}
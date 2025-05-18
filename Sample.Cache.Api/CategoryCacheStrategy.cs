// <copyright file="CategoryCacheStrategy.cs" project="Sample.Cache.Api">
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
///     Custom cache strategy for category-related endpoints
/// </summary>
public class CategoryCacheStrategy : ICacheStrategy
{
    private readonly ILogger<CategoryCacheStrategy> _logger;

    public CategoryCacheStrategy(ILogger<CategoryCacheStrategy> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<bool> ShouldCacheResponse(ActionExecutingContext context)
    {
        // Only cache GET requests
        if (!context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(false);

        _logger.LogDebug("CategoryCacheStrategy: Evaluating if response should be cached");

        // Cache all category GET requests
        return Task.FromResult(true);
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
        _logger.LogDebug("CategoryCacheStrategy: Invalidating related cache entries");

        // Get the category ID from the route data
        if (context.RouteData.Values.TryGetValue("id", out var idObj) &&
            int.TryParse(idObj?.ToString(), out var categoryId))
        {
            // 1. Invalidate specific category cache
            var categoryKey = $"/api/categories/{categoryId}|*";
            var categoryKeyToInvalidate = cacheManager.GetRedisKeys(categoryKey);

            if (categoryKeyToInvalidate.Any())
            {
                _logger.LogDebug("CategoryCacheStrategy: Invalidating cache for category ID {CategoryId}", categoryId);
                await cacheManager.DeleteAllAsync(categoryKeyToInvalidate.Select(k => k.ToString()));
            }

            // 2. Invalidate products in this category cache
            var productsInCategoryKey = $"/api/products/category/{categoryId}|*";
            var productsInCategoryKeysToInvalidate = cacheManager.GetRedisKeys(productsInCategoryKey);

            if (productsInCategoryKeysToInvalidate.Any())
            {
                _logger.LogDebug("CategoryCacheStrategy: Invalidating cache for products in category {CategoryId}",
                    categoryId);
                await cacheManager.DeleteAllAsync(productsInCategoryKeysToInvalidate.Select(k => k.ToString()));
            }

            // 3. Always invalidate popular categories cache if a category is updated
            var popularKey = "/api/categories/popular|*";
            var popularKeysToInvalidate = cacheManager.GetRedisKeys(popularKey);

            if (popularKeysToInvalidate.Any())
            {
                _logger.LogDebug("CategoryCacheStrategy: Invalidating popular categories cache");
                await cacheManager.DeleteAllAsync(popularKeysToInvalidate.Select(k => k.ToString()));
            }
        }
        else
        {
            // If no specific ID is found, invalidate all category caches
            var allCategoriesKey = "/api/categories*";
            var allKeysToInvalidate = cacheManager.GetRedisKeys(allCategoriesKey);

            if (allKeysToInvalidate.Any())
            {
                _logger.LogDebug("CategoryCacheStrategy: Invalidating all category caches ({Count} items)",
                    allKeysToInvalidate.Count());
                await cacheManager.DeleteAllAsync(allKeysToInvalidate.Select(k => k.ToString()));
            }
        }
    }
}
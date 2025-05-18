// <copyright file="UserSpecificCacheStrategy.cs" project="Cache">
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
///     User-specific caching strategy that varies cache by user identity.
/// </summary>
public class UserSpecificCacheStrategy : DefaultCacheStrategy
{
    /// <inheritdoc />
    public override Task<bool> ShouldCacheResponse(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(false);

        // Only cache for authenticated users
        return Task.FromResult(context.HttpContext.User.Identity?.IsAuthenticated == true);
    }

    /// <inheritdoc />
    public override async Task InvalidateRelatedCacheEntries(ActionExecutingContext context,
        ICacheManager<CacheEntry> cacheManager)
    {
        // First invalidate standard path-based cache entries
        await base.InvalidateRelatedCacheEntries(context, cacheManager);

        // Then invalidate user-specific cache entries if user is authenticated
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var userPattern = $"*|user-{userId}*";
                var keysToInvalidate = cacheManager.GetRedisKeys(userPattern);

                if (keysToInvalidate.Any())
                {
                    var logger = context.HttpContext.RequestServices.GetService<ILogger<UserSpecificCacheStrategy>>();
                    logger?.LogDebug("Invalidating {Count} user-specific cache entries for user: {UserId}",
                        keysToInvalidate.Count(), userId);
                    await cacheManager.DeleteAllAsync(keysToInvalidate.Select(k => k.ToString()));
                }
            }
        }
    }
}
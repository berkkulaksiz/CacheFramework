// <copyright file="CachingServiceCollectionExtensions.cs" project="Cache">
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
///     Extension methods for setting up caching services in an <see cref="IServiceCollection" />.
/// </summary>
public static class CachingServiceCollectionExtensions
{
    /// <summary>
    ///     Adds caching services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection" /> for chaining.</returns>
    public static IServiceCollection AddMicroFrameCaching(this IServiceCollection services)
    {
        return services.AddMicroFrameCaching(options => { });
    }

    /// <summary>
    ///     Adds caching services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection" /> for chaining.</returns>
    public static IServiceCollection AddMicroFrameCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddMicroFrameCaching(options => { configuration.GetSection("Caching").Bind(options); });
    }

    /// <summary>
    ///     Adds caching services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="configureOptions">The configure options callback.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection" /> for chaining.</returns>
    public static IServiceCollection AddMicroFrameCaching(
        this IServiceCollection services,
        Action<CachingOptions> configureOptions)
    {
        // Add options
        var options = new CachingOptions();
        configureOptions(options);
        services.AddSingleton(options);
        
        // Add memory cache for fallback and stats
        services.AddMemoryCache();

        // Add core services
        services.TryAddSingleton<ICacheKeyGenerator, DefaultCacheKeyGenerator>();
        services.TryAddSingleton<ICacheTimeoutProvider, DefaultCacheTimeoutProvider>();
        services.TryAddSingleton<ICacheMetrics, CacheMetrics>();

        // Add strategies
        services.TryAddTransient<ICacheStrategy, DefaultCacheStrategy>();
        services.TryAddTransient<UserSpecificCacheStrategy>();
        services.TryAddTransient<ContentBasedCacheStrategy>();

        // Add cache descriptor provider for OpenAPI
        if (options.EnableSwaggerDocumentation) services.AddSingleton<CacheDescriptorProvider>();

        return services;
    }

    /// <summary>
    ///     Adds Redis distributed cache services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="redisConnectionString">The Redis connection string.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection" /> for chaining.</returns>
    public static IServiceCollection AddRedisCaching(
        this IServiceCollection services,
        IRedisSettings redisSettings)
    {
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisSettings.ConnectionString;
            options.ConfigurationOptions.AllowAdmin = redisSettings.AllowAdmin;
            options.InstanceName = "MicroFrame:";
        });
        

        services.AddSingleton<IRedisDatabaseFactory, RedisDatabaseFactory>();
        services.AddSingleton(typeof(ICacheManager<>), typeof(RedisCacheManager<>));

        return services;
    }

    /// <summary>
    ///     Adds metrics for cache monitoring.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="usePrometheus">If true, Prometheus metrics will be used.</param>
    /// <returns>The same instance of the <see cref="IServiceCollection" /> for chaining.</returns>
    public static IServiceCollection AddCacheMetrics(
        this IServiceCollection services,
        bool usePrometheus = false)
    {
        if (usePrometheus)
            services.AddSingleton<ICacheMetrics, PrometheusMetrics>();
        else
            services.AddSingleton<ICacheMetrics, CacheMetrics>();

        return services;
    }
}
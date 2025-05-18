// <copyright file="Program.cs" project="Sample.Cache.Api">
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

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        Configure(app, app.Environment);

        // Seed sample data
        using (var scope = app.Services.CreateScope())
        {
            var dataService = scope.ServiceProvider.GetRequiredService<IProductDataService>();
            dataService.SeedData();
        }

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ContentBasedCacheStrategyOptions>(
            configuration.GetSection("Caching:ContentBasedStrategy"));
        
        // Add controllers
        services.AddControllers();

        // Add sample services
        services.AddSingleton<IProductDataService, ProductDataService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();

        // Add memory cache
        services.AddMemoryCache();

        // Add MicroFrame caching
        services.AddMicroFrameCaching(options =>
        {
            options.DefaultTimeToLiveSeconds = 60;
            options.EnableSwaggerDocumentation = true;
            options.EnableBackgroundRefresh = true;
        });

        // Add Redis caching (commented out as it requires Redis server)
        // Uncomment this if you have Redis server running
        var redisSettings = configuration.GetSection("Redis").Get<RedisSettings>();
        // Register Redis settings (mock implementation for demo)
        services.AddSingleton<IRedisSettings>(redisSettings);
        
        services.AddRedisCaching(redisSettings);

        // Add cache metrics
        services.AddCacheMetrics();

        // Add cache strategies
        services.AddTransient<ProductCacheStrategy>();
        services.AddTransient<CategoryCacheStrategy>();
        services.AddTransient<ContentBasedCacheStrategy>();
        
        // Configure Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sample Cache API",
                Version = "v1",
                Description = "Sample API demonstrating MicroFrame Caching capabilities"
            });

            // Uncomment when you have SwaggerCacheExtensions available
            // options.AddCacheDocumentation();
        });
    }

    private static void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Cache API v1");
            c.RoutePrefix = string.Empty; // Set Swagger UI at app root
        });

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
    }
}
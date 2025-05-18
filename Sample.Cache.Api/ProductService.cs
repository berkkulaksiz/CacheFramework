// <copyright file="ProductService.cs" project="Sample.Cache.Api">
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
///     Implementation of product service
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductDataService _dataService;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductDataService dataService, ILogger<ProductService> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        _logger.LogDebug("Fetching all products from data service");
        // Simulate async operation
        await Task.Delay(100);
        return _dataService.GetAllProducts();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        _logger.LogDebug("Fetching product with ID: {ProductId}", id);
        // Simulate async operation
        await Task.Delay(50);
        return _dataService.GetProductById(id);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        _logger.LogDebug("Fetching products for category ID: {CategoryId}", categoryId);
        // Simulate async operation
        await Task.Delay(150);
        return _dataService.GetProductsByCategory(categoryId);
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        _logger.LogDebug("Fetching featured products");
        // Simulate async operation
        await Task.Delay(75);
        return _dataService.GetFeaturedProducts();
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        _logger.LogDebug("Adding new product: {ProductName}", product.Name);
        // Simulate async operation
        await Task.Delay(200);
        return _dataService.AddProduct(product);
    }

    public async Task UpdateProductAsync(Product product)
    {
        _logger.LogDebug("Updating product with ID: {ProductId}", product.Id);
        // Simulate async operation
        await Task.Delay(150);
        _dataService.UpdateProduct(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        _logger.LogDebug("Deleting product with ID: {ProductId}", id);
        // Simulate async operation
        await Task.Delay(100);
        _dataService.DeleteProduct(id);
    }
}
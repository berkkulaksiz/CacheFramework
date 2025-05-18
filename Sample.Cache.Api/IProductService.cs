// <copyright file="IProductService.cs" project="Sample.Cache.Api">
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
///     Interface for product service
/// </summary>
public interface IProductService
{
    /// <summary>
    ///     Gets all products
    /// </summary>
    Task<IEnumerable<Product>> GetAllProductsAsync();

    /// <summary>
    ///     Gets a product by ID
    /// </summary>
    Task<Product> GetProductByIdAsync(int id);

    /// <summary>
    ///     Gets products by category ID
    /// </summary>
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);

    /// <summary>
    ///     Gets featured products
    /// </summary>
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();

    /// <summary>
    ///     Adds a new product
    /// </summary>
    Task<Product> AddProductAsync(Product product);

    /// <summary>
    ///     Updates an existing product
    /// </summary>
    Task UpdateProductAsync(Product product);

    /// <summary>
    ///     Deletes a product
    /// </summary>
    Task DeleteProductAsync(int id);
}
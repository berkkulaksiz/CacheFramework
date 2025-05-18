// <copyright file="IProductDataService.cs" project="Sample.Cache.Api">
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
///     Interface for data service that manages products and categories
/// </summary>
public interface IProductDataService
{
    // Product methods
    IEnumerable<Product> GetAllProducts();
    Product GetProductById(int id);
    IEnumerable<Product> GetProductsByCategory(int categoryId);
    IEnumerable<Product> GetFeaturedProducts();
    Product AddProduct(Product product);
    void UpdateProduct(Product product);
    void DeleteProduct(int id);

    // Category methods
    IEnumerable<Category> GetAllCategories();
    Category GetCategoryById(int id);
    IEnumerable<Category> GetPopularCategories();
    Category AddCategory(Category category);
    void UpdateCategory(Category category);
    void DeleteCategory(int id);

    // Seed data method
    void SeedData();
}
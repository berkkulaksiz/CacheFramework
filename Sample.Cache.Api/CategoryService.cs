// <copyright file="CategoryService.cs" project="Sample.Cache.Api">
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
///     Implementation of category service
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IProductDataService _dataService;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IProductDataService dataService, ILogger<CategoryService> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        _logger.LogDebug("Fetching all categories from data service");
        // Simulate async operation
        await Task.Delay(50);
        return _dataService.GetAllCategories();
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        _logger.LogDebug("Fetching category with ID: {CategoryId}", id);
        // Simulate async operation
        await Task.Delay(25);
        return _dataService.GetCategoryById(id);
    }

    public async Task<IEnumerable<Category>> GetPopularCategoriesAsync()
    {
        _logger.LogDebug("Fetching popular categories");
        // Simulate async operation
        await Task.Delay(75);
        return _dataService.GetPopularCategories();
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        _logger.LogDebug("Adding new category: {CategoryName}", category.Name);
        // Simulate async operation
        await Task.Delay(100);
        return _dataService.AddCategory(category);
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        _logger.LogDebug("Updating category with ID: {CategoryId}", category.Id);
        // Simulate async operation
        await Task.Delay(75);
        _dataService.UpdateCategory(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        _logger.LogDebug("Deleting category with ID: {CategoryId}", id);
        // Simulate async operation
        await Task.Delay(50);
        _dataService.DeleteCategory(id);
    }
}
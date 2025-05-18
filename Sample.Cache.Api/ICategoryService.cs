// <copyright file="ICategoryService.cs" project="Sample.Cache.Api">
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
///     Interface for category service
/// </summary>
public interface ICategoryService
{
    /// <summary>
    ///     Gets all categories
    /// </summary>
    Task<IEnumerable<Category>> GetAllCategoriesAsync();

    /// <summary>
    ///     Gets a category by ID
    /// </summary>
    Task<Category> GetCategoryByIdAsync(int id);

    /// <summary>
    ///     Gets popular categories
    /// </summary>
    Task<IEnumerable<Category>> GetPopularCategoriesAsync();

    /// <summary>
    ///     Adds a new category
    /// </summary>
    Task<Category> AddCategoryAsync(Category category);

    /// <summary>
    ///     Updates an existing category
    /// </summary>
    Task UpdateCategoryAsync(Category category);

    /// <summary>
    ///     Deletes a category
    /// </summary>
    Task DeleteCategoryAsync(int id);
}
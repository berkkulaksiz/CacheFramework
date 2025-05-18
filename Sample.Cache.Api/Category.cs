// <copyright file="Category.cs" project="Sample.Cache.Api">
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
///     Represents a category of products
/// </summary>
public class Category
{
    /// <summary>
    ///     Gets or sets the unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the category name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the category description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets whether the category is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    ///     Gets or sets the parent category ID (for hierarchical categories)
    /// </summary>
    public int? ParentCategoryId { get; set; }

    /// <summary>
    ///     Gets or sets the popularity score (higher means more popular)
    /// </summary>
    public int PopularityScore { get; set; }

    /// <summary>
    ///     Gets or sets the creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
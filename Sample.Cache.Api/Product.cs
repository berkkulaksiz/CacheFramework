// <copyright file="Product.cs" project="Sample.Cache.Api">
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
///     Represents a product in the catalog
/// </summary>
public class Product
{
    /// <summary>
    ///     Gets or sets the unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the product name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the product description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the product price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    ///     Gets or sets the product stock quantity
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    ///     Gets or sets whether the product is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    ///     Gets or sets the category ID
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    ///     Gets or sets the creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Gets or sets the last update date
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
// <copyright file="ProductDataService.cs" project="Sample.Cache.Api">
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
///     In-memory implementation of the data service
/// </summary>
public class ProductDataService : IProductDataService
{
    private readonly List<Category> _categories = new();
    private readonly object _lock = new();
    private readonly ILogger<ProductDataService> _logger;
    private readonly List<Product> _products = new();
    private int _nextCategoryId = 1;
    private int _nextProductId = 1;

    public ProductDataService(ILogger<ProductDataService> logger)
    {
        _logger = logger;
    }

    public IEnumerable<Product> GetAllProducts()
    {
        return _products.ToList(); // Return a copy to prevent modifications
    }

    public Product GetProductById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Product> GetProductsByCategory(int categoryId)
    {
        return _products.Where(p => p.CategoryId == categoryId).ToList();
    }

    public IEnumerable<Product> GetFeaturedProducts()
    {
        return _products.Where(p => p.IsFeatured).ToList();
    }

    public Product AddProduct(Product product)
    {
        lock (_lock)
        {
            product.Id = _nextProductId++;
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            _products.Add(product);
            return product;
        }
    }

    public void UpdateProduct(Product product)
    {
        lock (_lock)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                // Preserve original creation date
                product.CreatedAt = existingProduct.CreatedAt;
                product.UpdatedAt = DateTime.UtcNow;

                // Remove old and add updated
                _products.Remove(existingProduct);
                _products.Add(product);
            }
        }
    }

    public void DeleteProduct(int id)
    {
        lock (_lock)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null) _products.Remove(product);
        }
    }

    public IEnumerable<Category> GetAllCategories()
    {
        return _categories.ToList(); // Return a copy to prevent modifications
    }

    public Category GetCategoryById(int id)
    {
        return _categories.FirstOrDefault(c => c.Id == id);
    }

    public IEnumerable<Category> GetPopularCategories()
    {
        return _categories.Where(c => c.PopularityScore > 5).ToList();
    }

    public Category AddCategory(Category category)
    {
        lock (_lock)
        {
            category.Id = _nextCategoryId++;
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;
            _categories.Add(category);
            return category;
        }
    }

    public void UpdateCategory(Category category)
    {
        lock (_lock)
        {
            var existingCategory = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                // Preserve original creation date
                category.CreatedAt = existingCategory.CreatedAt;
                category.UpdatedAt = DateTime.UtcNow;

                // Remove old and add updated
                _categories.Remove(existingCategory);
                _categories.Add(category);
            }
        }
    }

    public void DeleteCategory(int id)
    {
        lock (_lock)
        {
            var category = _categories.FirstOrDefault(c => c.Id == id);
            if (category != null) _categories.Remove(category);
        }
    }

    public void SeedData()
    {
        lock (_lock)
        {
            if (_categories.Count > 0 || _products.Count > 0)
                // Data already seeded
                return;

            _logger.LogInformation("Seeding sample data");

            // Add categories
            var categories = new[]
            {
                new Category
                {
                    Name = "Electronics",
                    Description = "Electronic devices and gadgets",
                    IsActive = true,
                    PopularityScore = 10,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Clothing",
                    Description = "Apparel and accessories",
                    IsActive = true,
                    PopularityScore = 8,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Books",
                    Description = "Books and e-books",
                    IsActive = true,
                    PopularityScore = 6,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Home & Garden",
                    Description = "Home decor and gardening supplies",
                    IsActive = true,
                    PopularityScore = 4,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Sports",
                    Description = "Sports equipment and accessories",
                    IsActive = true,
                    PopularityScore = 7,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var category in categories) AddCategory(category);

            // Add products
            var products = new[]
            {
                new Product
                {
                    Name = "Smartphone X",
                    Description = "Latest smartphone with advanced features",
                    Price = 999.99m,
                    StockQuantity = 100,
                    IsFeatured = true,
                    CategoryId = 1, // Electronics
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Laptop Pro",
                    Description = "High-performance laptop for professionals",
                    Price = 1499.99m,
                    StockQuantity = 50,
                    IsFeatured = true,
                    CategoryId = 1, // Electronics
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Wireless Earbuds",
                    Description = "Premium wireless earbuds with noise cancellation",
                    Price = 149.99m,
                    StockQuantity = 200,
                    IsFeatured = false,
                    CategoryId = 1, // Electronics
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Designer Jeans",
                    Description = "Stylish designer jeans for all occasions",
                    Price = 89.99m,
                    StockQuantity = 75,
                    IsFeatured = false,
                    CategoryId = 2, // Clothing
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Summer Dress",
                    Description = "Lightweight summer dress perfect for hot days",
                    Price = 59.99m,
                    StockQuantity = 120,
                    IsFeatured = true,
                    CategoryId = 2, // Clothing
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Bestselling Novel",
                    Description = "The latest bestselling fiction novel",
                    Price = 24.99m,
                    StockQuantity = 300,
                    IsFeatured = true,
                    CategoryId = 3, // Books
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Cookbook Collection",
                    Description = "Collection of gourmet cooking recipes",
                    Price = 34.99m,
                    StockQuantity = 150,
                    IsFeatured = false,
                    CategoryId = 3, // Books
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Houseplant Set",
                    Description = "Set of 3 easy-care indoor houseplants",
                    Price = 49.99m,
                    StockQuantity = 80,
                    IsFeatured = false,
                    CategoryId = 4, // Home & Garden
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Running Shoes",
                    Description = "Professional running shoes with enhanced comfort",
                    Price = 129.99m,
                    StockQuantity = 90,
                    IsFeatured = true,
                    CategoryId = 5, // Sports
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Yoga Mat",
                    Description = "Premium non-slip yoga mat",
                    Price = 39.99m,
                    StockQuantity = 200,
                    IsFeatured = false,
                    CategoryId = 5, // Sports
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var product in products) AddProduct(product);

            _logger.LogInformation("Seeded {CategoryCount} categories and {ProductCount} products",
                _categories.Count, _products.Count);
        }
    }
}
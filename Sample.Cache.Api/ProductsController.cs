// <copyright file="ProductsController.cs" project="Sample.Cache.Api">
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

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductService _productService;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    ///     Gets all products with basic caching (60 seconds)
    /// </summary>
    [HttpGet]
    [Cached(60)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        _logger.LogInformation("Getting all products");
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    ///     Gets a product by ID with advanced caching (5 minutes)
    /// </summary>
    [HttpGet("{id}")]
    [Cached(300, CachePolicy.ApiPolicy)]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null) return NotFound();

        return Ok(product);
    }

    /// <summary>
    ///     Gets products by category with custom cache strategy
    /// </summary>
    [HttpGet("category/{categoryId}")]
    [Cached(300, typeof(ProductCacheStrategy))]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
    {
        _logger.LogInformation("Getting products for category ID: {CategoryId}", categoryId);
        var products = await _productService.GetProductsByCategoryAsync(categoryId);

        if (!products.Any()) return NotFound();

        return Ok(products);
    }

    /// <summary>
    ///     Gets featured products with high performance caching
    /// </summary>
    [HttpGet("featured")]
    [Cached(600, CachePolicy.HighPerformancePolicy)]
    public async Task<ActionResult<IEnumerable<Product>>> GetFeaturedProducts()
    {
        _logger.LogInformation("Getting featured products");
        var products = await _productService.GetFeaturedProductsAsync();
        return Ok(products);
    }

    /// <summary>
    ///     Creates a new product (invalidates cache)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        _logger.LogInformation("Creating new product");
        var createdProduct = await _productService.AddProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }

    /// <summary>
    ///     Updates a product (invalidates cache)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id) return BadRequest();

        _logger.LogInformation("Updating product with ID: {ProductId}", id);
        await _productService.UpdateProductAsync(product);
        return NoContent();
    }

    /// <summary>
    ///     Deletes a product (invalidates cache)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }
}
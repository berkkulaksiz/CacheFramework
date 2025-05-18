// <copyright file="CategoriesController.cs" project="Sample.Cache.Api">
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
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    ///     Gets all categories with basic caching
    /// </summary>
    [HttpGet]
    [Cached(120)]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        _logger.LogInformation("Getting all categories");
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    ///     Gets a category by ID with custom strategy
    /// </summary>
    [HttpGet("{id}")]
    [Cached(300, typeof(CategoryCacheStrategy))]
    public async Task<ActionResult<Category>> GetCategory(int id)
    {
        _logger.LogInformation("Getting category with ID: {CategoryId}", id);
        var category = await _categoryService.GetCategoryByIdAsync(id);

        if (category == null) return NotFound();

        return Ok(category);
    }

    /// <summary>
    ///     Gets popular categories with high performance caching
    /// </summary>
    [HttpGet("popular")]
    [Cached(600, CachePolicy.HighPerformancePolicy)]
    public async Task<ActionResult<IEnumerable<Category>>> GetPopularCategories()
    {
        _logger.LogInformation("Getting popular categories");
        var categories = await _categoryService.GetPopularCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    ///     Creates a new category (invalidates cache)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category category)
    {
        _logger.LogInformation("Creating new category");
        var createdCategory = await _categoryService.AddCategoryAsync(category);
        return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
    }

    /// <summary>
    ///     Updates a category (invalidates cache)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category category)
    {
        if (id != category.Id) return BadRequest();

        _logger.LogInformation("Updating category with ID: {CategoryId}", id);
        await _categoryService.UpdateCategoryAsync(category);
        return NoContent();
    }

    /// <summary>
    ///     Deletes a category (invalidates cache)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}
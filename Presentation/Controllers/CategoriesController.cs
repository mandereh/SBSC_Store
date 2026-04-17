using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    public CategoriesController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    
    /// <summary> 
    /// Gets the list of all Product Categories 
    /// </summary> 
    /// <returns>The product categories list</returns>
    [Authorize(Roles = "Admin, Customer")]
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategories(bool trackChanges)
    {
        var categories = await _serviceManager.CategoryService.GetAllCategoriesAsync(trackChanges: false);
        return Ok(categories);
    }

    /// <summary>
    /// Get a single category by its Id
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>A single category by Id</returns>
    [Authorize(Roles = "Admin, Customer")]
    [HttpGet("{id:guid}",  Name = "CategoryById")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _serviceManager.CategoryService.GetCategoryAsync(id, trackChanges: false);
        return Ok(category);
    }

    /// <summary> 
    /// Creates a newly created category 
    /// </summary> 
    /// <param name="category"></param> 
    /// <returns>A newly created category</returns> 
    /// <response code="201">Returns the newly created item</response> 
    /// <response code="400">If the item is null</response> 
    /// <response code="422">If the model is invalid</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryForCreationDto categoryForCreationDto)
    {
        if(categoryForCreationDto == null)
            return BadRequest("CategoryForCreationDto object is null");
        if(!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        var category = await _serviceManager.CategoryService.CreateCategoryAsync(categoryForCreationDto);
        return CreatedAtRoute("CategoryById", new { id = category.Id }, category);
    }

    
    /// <summary>
    /// Gets a collection of categories by their Ids
    /// </summary>
    /// <param name="ids">Collection of category Ids</param>
    /// <returns>A collection of categories matching the provided Ids</returns>
    [Authorize(Roles = "Admin, Customer")]
    [HttpGet("collection/({ids})", Name = "CategoryCollection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategoryCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    {
        var categories = await _serviceManager.CategoryService.GetByIdsAsync(ids, trackChanges: false);
        return Ok(categories);
    }

    /// <summary>
    /// Creates a collection of categories.
    /// </summary>
    /// <param name="categoryForCreationDtos">The categories to create.</param>
    /// <returns>A <see cref="CreatedAtRouteResult"/> containing the created categories.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("collection")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategoryCollection(
        [FromBody] IEnumerable<CategoryForCreationDto> categoryForCreationDtos)
    {
        var result = await _serviceManager.CategoryService.CreateCategoryCollectionAsync(categoryForCreationDtos);
        return CreatedAtRoute("CategoryCollection", new {result.ids}, result.categories);
    }

    /// <summary>
    /// Deletes a category by its Id.
    /// </summary>
    /// <param name="id">The category Id.</param>
    /// <returns>A \`204 No Content\` response when deletion succeeds.</returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _serviceManager.CategoryService.DeleteCategoryAsync(id, trackChanges: false);
        return NoContent();
    }

    /// <summary>
    /// Updates a category by its Id.
    /// </summary>
    /// <param name="categoryId">The category Id.</param>
    /// <param name="categoryForUpdateDto">The category data to update.</param>
    /// <returns>A `204 No Content` response when the update succeeds.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] CategoryForUpdateDto categoryForUpdateDto)
    {
        if(categoryForUpdateDto == null)
            return BadRequest("CategoryForUpdateDto object is null");
        await _serviceManager.CategoryService.UpdateCategoryAsync(categoryId,categoryForUpdateDto,true);
        return NoContent();
    }
}
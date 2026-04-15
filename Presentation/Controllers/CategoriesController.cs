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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategories(bool trackChanges)
    {
        var categories = await _serviceManager.CategoryService.GetAllCategoriesAsync(trackChanges: false);
        return Ok(categories);
    }

    [HttpGet("{id:guid}",  Name = "CategoryById")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        var category = await _serviceManager.CategoryService.GetCategoryAsync(id, trackChanges: false);
        return Ok(category);
    }

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

    [HttpGet("collection/({ids})", Name = "CategoryCollection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCategoryCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    {
        var categories = await _serviceManager.CategoryService.GetByIdsAsync(ids, trackChanges: false);
        return Ok(categories);
    }

    [HttpPost("collection")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategoryCollection(
        [FromBody] IEnumerable<CategoryForCreationDto> categoryForCreationDtos)
    {
        var result = await _serviceManager.CategoryService.CreateCategoryCollectionAsync(categoryForCreationDtos);
        return CreatedAtRoute("CategoryCollection", new {result.ids}, result.categories);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _serviceManager.CategoryService.DeleteCategoryAsync(id, trackChanges: false);
        return NoContent();
    }

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
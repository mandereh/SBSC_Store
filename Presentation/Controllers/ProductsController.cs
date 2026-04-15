using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/categories/{categoryId}/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    public ProductsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsForCategory(Guid categoryId)
    {
        var products = await _serviceManager.ProductService.GetProductsAsync(categoryId, trackChanges: false);
        return Ok(products);
    }

    [HttpGet("{productId:guid}", Name = "GetProductForCategory")]
    public async Task<IActionResult> GetProductForCategory(Guid categoryId, Guid productId)
    {
        var product = await _serviceManager.ProductService.GetProductAsync(categoryId, productId, trackChanges: false);
        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProductForCategory(Guid categoryId, [FromBody]ProductForCreationDto productForCreationDto)
    {
        if (productForCreationDto == null)
            return BadRequest("productForCreationDto is null");
        if(!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        var product = await _serviceManager.ProductService.CreateProductForCategoryAsync(categoryId, productForCreationDto, trackChanges: false);
        return CreatedAtRoute("GetProductForCategory", new {categoryId, productId = product.Id }, product);
    }

    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductForCategory(Guid categoryId, Guid productId)
    {
        await _serviceManager.ProductService.DeleteProductForCategoryAsync(categoryId: categoryId, productId: productId, trackChanges: false);
        return NoContent();
    }
    [HttpPut("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProductForCategory(Guid categoryId, Guid productId,
        [FromBody]ProductForUpdateDto productForUpdateDto)
    {
        if (productForUpdateDto == null)
            return BadRequest("productForUpdateDto object is null");
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        await _serviceManager.ProductService.UpdateProductForCategoryAsync(categoryId,productId, productForUpdateDto,false,true);
        return NoContent();
    }
    
    [HttpPatch("{productId:guid}")]
    public async Task<IActionResult> PartiallyUpdateProductForCategory(Guid categoryId, Guid productId,
        [FromBody] JsonPatchDocument<ProductForUpdateDto> productForUpdateDto)
    {
        if (productForUpdateDto == null)
            return BadRequest("productForUpdateDto sent from the client is null");
        var result = await _serviceManager.ProductService.GetProductForPatchAsync(categoryId, productId, false, true);
        productForUpdateDto.ApplyTo(result.productForUpdateDto, ModelState);
        TryValidateModel(result.productForUpdateDto);
        if (!ModelState.IsValid) 
            return UnprocessableEntity(ModelState); 
        await _serviceManager.ProductService.SaveChangesForPatchAsync(result.productForUpdateDto, result.product);
        return NoContent();
    }
}
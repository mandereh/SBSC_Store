using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Presentation.Controllers;

[Route("api/categories/{categoryId}/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    public ProductsController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    
    /// <summary>
    /// Retrieves all products for the specified category.
    /// Use the <c>pageNumber</c> and <c>pageSize</c> query string parameters to control paging,
    /// <c>minPrice</c> and <c>maxPrice</c> to filter by price range,
    /// and <c>searchTerm</c> to search products by name or description.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="productParameters">The query parameters used for paging, price range filtering, and search.</param>
    /// <returns>An <see cref="IActionResult"/> containing the paged list of products for the specified category.</returns>
    [Authorize(Roles = "Admin, Customer")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsForCategory(Guid categoryId, [FromQuery] ProductParameters  productParameters)
    {
        var pagedResult = await _serviceManager.ProductService.GetProductsAsync(categoryId, productParameters, trackChanges: false);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.products);
    }

    /// <summary>
    /// Retrieves a single product by its identifier for the specified category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category that contains the product.</param>
    /// <param name="productId">The unique identifier of the product to retrieve.</param>
    /// <returns>An <see cref="IActionResult"/> containing the requested product DTO when found.</returns>
    [Authorize(Roles = "Admin, Customer")]
    [HttpGet("{productId:guid}", Name = "GetProductForCategory")]
    public async Task<IActionResult> GetProductForCategory(Guid categoryId, Guid productId)
    {
        var product = await _serviceManager.ProductService.GetProductAsync(categoryId, productId, trackChanges: false);
        return Ok(product);
    }

    /// <summary>
    /// Admin Creates a new product in the specified category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category to create the product in.</param>
    /// <param name="productForCreationDto">DTO containing the product data to create.</param>
    /// <param name="image"></param>
    /// <returns>An <see cref="IActionResult"/> that returns 201 Created with the created product DTO.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPost("/api/admin/products")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProductForCategory(Guid categoryId, [FromForm]ProductForCreationDto productForCreationDto, IFormFile? image)
    {
        if (productForCreationDto == null)
            return BadRequest("productForCreationDto is null");
        if(!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        var product = await _serviceManager.ProductService.CreateProductForCategoryAsync(categoryId, productForCreationDto,image, trackChanges: false);
        return CreatedAtRoute("GetProductForCategory", new {categoryId, productId = product.Id }, product);
    }

    /// <summary>
    /// Admin Deletes the specified product from the given category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category that contains the product.</param>
    /// <param name="productId">The unique identifier of the product to delete.</param>
    /// <returns>An <see cref="IActionResult"/> that returns 204 NoContent on success or 404 NotFound if not found.</returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("/api/admin/products/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductForCategory(Guid categoryId, Guid productId)
    {
        await _serviceManager.ProductService.DeleteProductForCategoryAsync(categoryId: categoryId, productId: productId, trackChanges: false);
        return NoContent();
    }

    /// <summary>
    /// Admin Updates a product for the specified category using a JSON Patch document.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category that contains the product.</param>
    /// <param name="productId">The unique identifier of the product to update.</param>
    /// <param name="productForUpdateDto">A JSON Patch document describing the partial updates.</param>
    /// <param name="image"></param>
    /// <returns>An <see cref="IActionResult"/> that returns 204 NoContent on success, 400 BadRequest for invalid input, or 422 UnprocessableEntity for model validation errors.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("/api/admin/products/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProductForCategory(Guid categoryId, Guid productId, [FromForm]ProductForUpdateDto productForUpdateDto, IFormFile? image)
    {
        if (productForUpdateDto == null)
            return BadRequest("productForUpdateDto object is null");
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        await _serviceManager.ProductService.UpdateProductForCategoryAsync(categoryId,productId, productForUpdateDto,false,true, image);
        return NoContent();
    }
    
    /// <summary>
    /// Admin Partially updates a product for the specified category using a JSON Patch document.
    /// Applies the patch to a ProductForUpdateDto and persists changes if valid.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category that contains the product.</param>
    /// <param name="productId">The unique identifier of the product to patch.</param>
    /// <param name="productForUpdateDto">A <see cref="JsonPatchDocument{ProductForUpdateDto}"/> describing the partial updates.</param>
    /// <returns>An <see cref="IActionResult"/> that returns 204 NoContent on success, 400 BadRequest for invalid input, or 422 UnprocessableEntity for model validation errors.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPatch("/api/admin/products/{productId:guid}")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
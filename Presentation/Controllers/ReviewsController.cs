using System.Security.Claims;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/products/{productId}/reviews")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IServiceManager  _serviceManager;
    public ReviewsController(IServiceManager serviceManager) => _serviceManager = serviceManager;
    
    
    /// <summary>
    /// Retrieves all reviews for the specified product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <returns>An <see cref="IActionResult"/> containing the list of reviews for the specified product.</returns>
    [HttpGet(Name = "GetReviewsForProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReviewsForProduct(Guid productId)
    {
        var reviews = await _serviceManager.ReviewService.GetProductReviewsAsync(productId);
        return Ok(reviews);
    }

    /// <summary>
    /// Creates a new review for the specified product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="reviewForCreationDto">The review to create.</param>
    /// <returns>An <see cref="IActionResult"/> containing the created review.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReviewForProduct(Guid  productId, [FromBody] ReviewForCreationDto reviewForCreationDto)
    {
        if(reviewForCreationDto == null)
            return BadRequest("reviewForCreationDto is null");
        if(!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        var review = await _serviceManager.ReviewService.CreateReviewAsync(productId, reviewForCreationDto, userId);
        return CreatedAtRoute("GetReviewsForProduct", new { productId = productId }, review);
        
    }

    /// <summary>
    /// Retrieves the average rating for the specified product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <returns>An <see cref="IActionResult"/> containing the average rating for the specified product.</returns>
    [HttpGet("average")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAverageRatingForProduct(Guid productId)
    {
        if (productId == Guid.Empty)
            return BadRequest("Product ID is empty");
        var averageRating = await _serviceManager.ReviewService.GetAverageRatingAsync(productId);
        return Ok(new { ProductId = productId, AverageRating = averageRating });
    }

    /// <summary>
    /// Deletes the specified review for the specified product.
    /// </summary>
    /// <param name="reviewId">The unique identifier of the review.</param>
    /// <returns>An <see cref="IActionResult"/> indicating whether the deletion was successful.</returns>
    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReviewForProduct(Guid reviewId)
    {
        if (reviewId == Guid.Empty)
            return BadRequest("Review ID is empty");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        await _serviceManager.ReviewService.DeleteReviewAsync(reviewId, userId);

        return NoContent();
    }

}
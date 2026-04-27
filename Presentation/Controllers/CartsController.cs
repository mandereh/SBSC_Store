using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/carts")]
[ApiController]
public class CartsController : ControllerBase
{
    private readonly IServiceManager  _serviceManager;
    
    public CartsController(IServiceManager serviceManager) => _serviceManager = serviceManager;
    
    
    
    /// <summary>
    /// Gets the cart for the authenticated user.
    /// </summary>
    /// <returns>The cart if successful, Unauthorized if user ID not found in claims</returns>
    [HttpGet(Name = "GetCart")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCartForUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        var cart = await _serviceManager.CartService.GetUserCartAsync(userId);
        return Ok(cart);
    }
    
    /// <summary>
    /// Adds a product to user's cart
    /// </summary>
    /// <param name="addToCartDto"></param>
    /// <returns></returns>
    /// <response code="201">Returns the updated cart</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authorized</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddCartItemToCart([FromBody] AddToCartDto addToCartDto)
    {
        if(addToCartDto == null)
            return BadRequest("AddToCartDto cannot be null");
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        var cart = await _serviceManager.CartItemService.AddToCartItemToCartAsync(userId, addToCartDto);
        return Ok(cart);
    }
    
    /// <summary>
    /// Updates a product in user's cart
    /// </summary>
    /// <param name="updateCartItemDto"></param>
    /// <returns> Returns the updated cart</returns>
    /// <response code="200">Returns the updated cart</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authorized</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateCartItemForCart([FromBody] UpdateCartItemDto updateCartItemDto)
    {
        if(updateCartItemDto == null)
            return BadRequest("UpdateCartItemDto cannot be null");
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        var cart = await _serviceManager.CartItemService.UpdateCartItemAsync(userId, updateCartItemDto);
        return Ok(cart);
    }
    
    /// <summary>
    /// Deletes a product from user's cart
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    /// <response code="200">Returns the updated cart</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the user is not authorized</response>
    [HttpDelete("{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteCartItemFromCart(Guid productId)
    {
        if(productId == Guid.Empty)
            return BadRequest("Product ID cannot be empty");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        var cart = await _serviceManager.CartItemService.DeleteCartItemAsync(userId, productId);
        return Ok(cart);
    }
    
    /// <summary>
    /// Increases the quantity of a product in user's cart
    /// </summary>
    /// <param name="productId"></param>
    /// <returns>Returns the updated cart</returns>
    [HttpPut("{productId}/increase")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> IncreaseCartItemQuantity(Guid productId)
    {
        if(productId == Guid.Empty)
            return BadRequest("Product ID cannot be empty");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        var cart = await _serviceManager.CartItemService.IncreaseCartItemQuantityAsync(userId, productId);
        return Ok(cart);
    }
    
    /// <summary>
    /// Decreases the quantity of a product in user's cart
    /// </summary>
    /// <param name="productId"></param>
    /// <returns> Returns the updated cart</returns>
    [HttpPut("{productId}/decrease")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DecreaseCartItemQuantity(Guid productId)
    {
        if(productId == Guid.Empty)
            return BadRequest("Product ID cannot be empty");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        var cart = await _serviceManager.CartItemService.DecreaseCartItemQuantityAsync(userId, productId);
        return Ok(cart);
    }

    /// <summary>
    /// Clears the cart for the authenticated user.
    /// </summary>
    /// <returns> No content if successful, Unauthorized if user ID not found in claims</returns>
    [HttpDelete("clear")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ClearCartForUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        await _serviceManager.CartService.ClearCartAsync(userId);
        return NoContent();
    }
    
    
    /// <summary>
    /// Deletes the cart for the authenticated user.
    /// </summary>
    /// <returns> No content if successful, Unauthorized if user ID not found in claims</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteCartForUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");
        await _serviceManager.CartService.DeleteCartAsync(userId);
        return NoContent();
    }
    
}
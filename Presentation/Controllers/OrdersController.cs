using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace Presentation.Controllers;

[Route("api/orders")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public OrdersController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    /// <summary>
    /// Places a new order for the authenticated user.
    /// </summary>
    /// <returns>An IActionResult indicating the result of the order placement.</returns>
    [HttpPost(Name = "PlaceOrder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PlaceOrder()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var result = await _serviceManager.OrderService.PlaceOrderAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Gets the orders for the authenticated user.
    /// </summary>
    /// <returns>An IActionResult containing the list of orders for the user.</returns>
    [HttpGet(Name = "GetUserOrders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserOrders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var orders = await _serviceManager.OrderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    /// <summary>
    /// Gets a specific order by order number for the authenticated user.
    /// </summary>
    /// <param name="orderNumber">The order number of the order to retrieve.</param>
    /// <returns>An IActionResult containing the order details or a 404 Not Found status if the order is not found.</returns>
    [HttpGet("{orderNumber}", Name = "GetOrderByNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrderByNumber(string orderNumber)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in claims");

        var order = await _serviceManager.OrderService.GetOrderByNumberAsync(orderNumber, userId);
        return Ok(order);
    }
}
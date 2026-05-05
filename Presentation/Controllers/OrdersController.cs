using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.RequestFeatures;

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
    /// Gets all orders for the admin dashboard.
    /// </summary>
    /// <param name="orderParameters">The order parameters for filtering and sorting the orders.</param>
    /// <returns>An IActionResult containing the paged result of orders or an error status.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("/api/admin/orders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllOrdersForAdmin([FromQuery] OrderParameters orderParameters)
    {
        var pagedResult = await _serviceManager.OrderService.GetOrdersAsync(orderParameters);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
        return Ok(pagedResult.orders);
    }
    
    /// <summary>
    /// Marks an order as shipped by its order number for the admin dashboard.
    /// </summary>
    /// <param name="orderNumber">The order number of the order to mark as shipped.</param>
    /// <returns>An IActionResult indicating the result of the order marking as shipped.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("/api/orders/{orderNumber}/ship")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkOrderAsShipped(string orderNumber)
    {
        var result = await _serviceManager.OrderService.MarkOrderAsShippedAsync(orderNumber);
        return Ok(result);
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
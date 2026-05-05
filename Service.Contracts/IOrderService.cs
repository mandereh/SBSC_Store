using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts;

public interface IOrderService
{
    /// <summary>
    /// Places an order for the given user.
    /// </summary>
    /// <param name="userId"> The user ID. </param>
    /// <returns> The place order response DTO </returns>
    Task<PlaceOrderResponseDto> PlaceOrderAsync(string userId);
    
    /// <summary>
    /// Verifies the payment and completes the order.
    /// </summary>
    /// <param name="paymentReference"> The payment reference. </param>
    /// <returns> The Placed order DTO. </returns>
    Task<OrderDto> VerifyPaymentAndCompleteOrderAsync(string paymentReference);
    /// <summary>
    /// Gets all orders for the given user.
    /// </summary>
    /// <param name="userId"> The user ID. </param>
    /// <returns></returns>
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
    /// <summary>
    /// Gets an order by its number for the given user.
    /// </summary>
    /// <param name="orderNumber"> The order number. </param>
    /// <param name="userId"> The user ID. </param>
    /// <returns> The order DTO. </returns>
    Task<OrderDto> GetOrderByNumberAsync(string orderNumber, string userId);
    /// <summary>
    /// Processes a webhook request.
    /// </summary>
    /// <param name="rawBody"> The raw request body. </param>
    /// <param name="signature"> The request signature. </param>
    /// <returns> The Order DTO. </returns>
    Task<OrderDto> ProcessWebhookAsync(string rawBody, string? signature);
    
    Task<(IEnumerable<OrderDto> orders, MetaData metaData)> GetOrdersAsync(OrderParameters orderParameters);
    Task<OrderDto> MarkOrderAsShippedAsync(string orderNumber);
}
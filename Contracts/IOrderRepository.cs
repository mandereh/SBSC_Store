using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IOrderRepository
{
    Task<Order?> GetOrderByPaymentReferenceAsync(string reference, bool trackChanges);
    Task<Order?> GetOrderByNumberAsync(string orderNumber, bool trackChanges);
    Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId, bool trackChanges);
    Task<PagedList<Order>> GetOrdersAsync(OrderParameters orderParameters, bool trackChanges);
    Task<bool> UserHasPurchasedProductAsync(string userId, Guid productId);
    void CreateOrder(Order order);
    void UpdateOrder(Order order);
}
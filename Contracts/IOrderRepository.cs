using Entities.Models;

namespace Contracts;

public interface IOrderRepository
{
    Task<Order?> GetOrderByPaymentReferenceAsync(string reference, bool trackChanges);
    Task<Order?> GetOrderByNumberAsync(string orderNumber, bool trackChanges);
    Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId, bool trackChanges);
    void CreateOrder(Order order);
    void UpdateOrder(Order order);
}
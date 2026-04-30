using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    public async Task<Order?> GetOrderByPaymentReferenceAsync(string reference, bool trackChanges) =>
        await FindByCondition(o => o.PaymentReference == reference, trackChanges)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync();

    public async Task<Order?> GetOrderByNumberAsync(string orderNumber, bool trackChanges) =>
        await FindByCondition(o => o.OrderNumber == orderNumber, trackChanges)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId, bool trackChanges) =>
        await FindByCondition(o => o.UserId == userId, trackChanges)
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public void CreateOrder(Order order) => Create(order);
    public void UpdateOrder(Order order) => Update(order);
}
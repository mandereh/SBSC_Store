using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

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
    
    public async Task<PagedList<Order>> GetOrdersAsync(OrderParameters orderParameters, bool trackChanges)
    {
        IQueryable<Order> query = FindAll(trackChanges)
            .Include(o => o.OrderItems)
            .Include(o => o.User);

        query = orderParameters.SortOrder?.ToLower() == "asc"
            ? query.OrderBy(o => o.CreatedAt)
            : query.OrderByDescending(o => o.CreatedAt);

        var orders = await query.ToListAsync();
        return PagedList<Order>.ToPagedList(orders, orderParameters.PageNumber, orderParameters.PageSize);
    }

    public async Task<bool> UserHasPurchasedProductAsync(string userId, Guid productId) =>
        await FindByCondition(order => order.UserId == userId, trackChanges: false)
            .AnyAsync(order => order.OrderItems.Any(orderItem => orderItem.ProductId == productId));
    public void CreateOrder(Order order) => Create(order);
    public void UpdateOrder(Order order) => Update(order);
}
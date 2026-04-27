namespace Contracts;

public interface IRepositoryManager
{
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IReviewRepository ReviewRepository { get; }
    ICartRepository CartRepository { get; }
    ICartItemRepository CartItemRepository { get; }
    Task SaveAsync();
}
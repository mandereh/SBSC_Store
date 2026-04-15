namespace Contracts;

public interface IRepositoryManager
{
    IProductRepository ProductRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    Task SaveAsync();
}
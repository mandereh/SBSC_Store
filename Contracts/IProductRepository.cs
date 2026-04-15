using Entities.Models;

namespace Contracts;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId, bool trackChanges);
    Task<Product?> GetProductAsync(Guid categoryId, Guid productId, bool trackChanges);
    void CreateProductForCategory(Guid categoryId, Product product);
    void DeleteProduct(Product product);
}
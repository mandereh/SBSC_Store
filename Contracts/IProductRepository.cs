using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts;

public interface IProductRepository
{
    Task<PagedList<Product>> GetProductsAsync(Guid categoryId, ProductParameters productParameters, bool trackChanges);
    Task<Product?> GetProductAsync(Guid categoryId, Guid productId, bool trackChanges);
    void CreateProductForCategory(Guid categoryId, Product product);
    void DeleteProduct(Product product);
}
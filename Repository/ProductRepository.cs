using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    public async Task<IEnumerable<Product>> GetProductsAsync(Guid categoryId, bool trackChanges) =>
    await FindByCondition(product => product.CategoryId.Equals(categoryId), trackChanges)
        .OrderBy(product => product.Name)
        .ToListAsync();
    
    public async Task<Product?> GetProductAsync(Guid categoryId, Guid productId, bool trackChanges) =>
    await FindByCondition(product => product.CategoryId.Equals(categoryId) && product.Id.Equals(productId), trackChanges)
        .SingleOrDefaultAsync();

    public void CreateProductForCategory(Guid categoryId, Product product)
    {
        product.CategoryId = categoryId;
        Create(product);
    }

    public void DeleteProduct(Product product) => Delete(product);
}
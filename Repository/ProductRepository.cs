using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;

namespace Repository;

public class ProductRepository : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    public async Task<PagedList<Product>> GetProductsAsync(Guid categoryId, ProductParameters productParameters, bool trackChanges)
    {
        var products = await FindByCondition(product => product.CategoryId.Equals(categoryId), trackChanges)
            .FilterProducts(productParameters.MinPrice, productParameters.MaxPrice)
            .Search(productParameters.SearchTerm)
            .OrderBy(product => product.Name)
            .ToListAsync();
        return PagedList<Product> 
            .ToPagedList(products, productParameters.PageNumber, 
                productParameters.PageSize); 
    }

    public async Task<Product?> GetProductAsync(Guid categoryId, Guid productId, bool trackChanges) =>
    await FindByCondition(product => product.CategoryId.Equals(categoryId) && product.Id.Equals(productId), trackChanges)
        .SingleOrDefaultAsync();
    
    public async Task<Product?> GetProductByIdAsync(Guid productId, bool trackChanges) =>
        await FindByCondition(product => product.Id.Equals(productId), trackChanges)
            .SingleOrDefaultAsync();

    public void CreateProductForCategory(Guid categoryId, Product product)
    {
        product.CategoryId = categoryId;
        Create(product);
    }

    public void DeleteProduct(Product product) => Delete(product);
    
   
   public void UpdateProductImage(Guid productId, string imageUrl)
   {
       var product = FindByCondition(p => p.Id.Equals(productId), trackChanges: true)
           .SingleOrDefault();
       if (product != null)
       {
           product.ImageUrl = imageUrl;
           Update(product);  // This marks the entity as modified
       }
   }
}
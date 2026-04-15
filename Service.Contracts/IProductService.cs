using Entities.Models;
using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(Guid categoryId, bool trackChanges);
    Task<ProductDto?> GetProductAsync(Guid categoryId, Guid productId, bool trackChanges);
    
    Task<ProductDto> CreateProductForCategoryAsync(Guid categoryId, ProductForCreationDto productForCreationDto, bool trackChanges);
    Task DeleteProductForCategoryAsync(Guid categoryId, Guid productId, bool trackChanges);

    Task UpdateProductForCategoryAsync(Guid categoryId, Guid productId, ProductForUpdateDto productForUpdateDto,
        bool catTrackChanges, bool proTrackChanges);
    Task<(ProductForUpdateDto productForUpdateDto, Product product)> GetProductForPatchAsync(Guid categoryId, Guid productId, bool catTrackChanges,  bool proTrackChanges);
    Task SaveChangesForPatchAsync(ProductForUpdateDto productForUpdateDto, Product product);
}
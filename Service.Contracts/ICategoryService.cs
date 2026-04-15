using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>>  GetAllCategoriesAsync(bool trackChanges);
    Task<CategoryDto> GetCategoryAsync(Guid categoryId, bool trackChanges);
    Task<CategoryDto> CreateCategoryAsync(CategoryForCreationDto category);
    Task<IEnumerable<CategoryDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
    Task<(IEnumerable<CategoryDto> categories, string ids)> CreateCategoryCollectionAsync (IEnumerable<CategoryForCreationDto> categoryCollection);
    Task DeleteCategoryAsync(Guid categoryId, bool trackChanges);
    Task  UpdateCategoryAsync(Guid categoryId, CategoryForUpdateDto categoryForUpdateDto,  bool trackChanges);

}
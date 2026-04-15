using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
{
    public CategoryRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges) => 
        await FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<Category?> GetCategoryAsync(Guid categoryId, bool trackChanges) =>
        await FindByCondition(category => category.Id.Equals(categoryId), trackChanges).SingleOrDefaultAsync();
    
    public void CreateCategory(Category category) => Create(category);
    
    public async Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
    await FindByCondition(category => ids.Contains(category.Id), trackChanges).ToListAsync();

    public void DeleteCategory(Category category) => Delete(category);
}
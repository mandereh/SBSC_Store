using Contracts;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext; 
    private readonly Lazy<ICategoryRepository> _categoryRepository; 
    private readonly Lazy<IProductRepository> _productRepository; 
 
    public RepositoryManager(RepositoryContext repositoryContext) 
    { 
        _repositoryContext = repositoryContext; 
        _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(repositoryContext)); 
        _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(repositoryContext)); 
    } 
 
    public ICategoryRepository CategoryRepository => _categoryRepository.Value; 
    public IProductRepository ProductRepository => _productRepository.Value; 
 
    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync(); 
}
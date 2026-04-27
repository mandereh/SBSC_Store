using Contracts;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext; 
    private readonly Lazy<ICategoryRepository> _categoryRepository; 
    private readonly Lazy<IProductRepository> _productRepository; 
    private readonly Lazy<IReviewRepository> _reviewRepository;
    private readonly Lazy<ICartRepository> _cartRepository;
    private readonly Lazy<ICartItemRepository> _cartItemRepository;
 
    public RepositoryManager(RepositoryContext repositoryContext) 
    { 
        _repositoryContext = repositoryContext; 
        _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(repositoryContext)); 
        _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(repositoryContext)); 
        _reviewRepository = new Lazy<IReviewRepository>(() => new ReviewRepository(repositoryContext));
        _cartRepository = new Lazy<ICartRepository>(() => new CartRepository(repositoryContext));
        _cartItemRepository = new Lazy<ICartItemRepository>(() => new CartItemRepository(repositoryContext));
    } 
 
    public ICategoryRepository CategoryRepository => _categoryRepository.Value; 
    public IProductRepository ProductRepository => _productRepository.Value; 
    public IReviewRepository ReviewRepository => _reviewRepository.Value;
    public ICartRepository CartRepository => _cartRepository.Value;
    public ICartItemRepository CartItemRepository => _cartItemRepository.Value;
    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync(); 
}
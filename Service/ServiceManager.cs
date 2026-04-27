using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;

namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<ICategoryService> _categoryService;
    private readonly Lazy<IProductService> _productService;
    private readonly Lazy<IAuthenticationService> _authenticationService;
    private readonly Lazy<ICartService> _cartService;
    private readonly Lazy<ICartItemService> _cartItemService;
    private readonly Lazy<IReviewService> _reviewService;

    public ServiceManager(
        IRepositoryManager repositoryManager, 
        ILoggerManager logger, 
        IMapper mapper,
        UserManager<User> userManager,
        IConfiguration configuration,
        IFileServiceFactory fileServiceFactory
        )
    {
        _categoryService = new Lazy<ICategoryService>(() => new CategoryService(repositoryManager, logger, mapper));
        _productService = new Lazy<IProductService>(() => new ProductService(repositoryManager, logger, mapper, fileServiceFactory));
        _authenticationService =
            new Lazy<IAuthenticationService>(() =>
                new AuthenticationService(logger, mapper, userManager, configuration));
        _cartService = new Lazy<ICartService>(() => new CartService(repositoryManager, logger, mapper));
        _cartItemService = new Lazy<ICartItemService>(() => 
            new CartItemService(repositoryManager, logger, mapper));
        _reviewService = new Lazy<IReviewService>(() => new ReviewService(repositoryManager, logger, mapper));
    }

    public ICategoryService CategoryService => _categoryService.Value;
    public IProductService ProductService => _productService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
    public IReviewService ReviewService => _reviewService.Value;
    public ICartService CartService => _cartService.Value;
    public ICartItemService CartItemService => _cartItemService.Value;


}
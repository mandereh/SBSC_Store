namespace Service.Contracts;

public interface IServiceManager
{
    ICategoryService CategoryService { get; }
    IProductService ProductService { get; }
    IReviewService ReviewService { get; }
    ICartService CartService { get; }
    ICartItemService CartItemService { get; }
    IAuthenticationService AuthenticationService { get; }
}
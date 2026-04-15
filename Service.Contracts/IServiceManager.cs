namespace Service.Contracts;

public interface IServiceManager
{
    ICategoryService CategoryService { get; }
    IProductService ProductService { get; }
}
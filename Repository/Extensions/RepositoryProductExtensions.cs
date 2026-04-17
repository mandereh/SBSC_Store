using Entities.Models;

namespace Repository.Extensions;

public static class RepositoryProductExtensions
{
    public static IQueryable<Product> FilterProducts(this IQueryable<Product> products, decimal minPrice,
        decimal maxPrice) =>  products.Where(product => (product.Price >= minPrice && product.Price <= maxPrice));

    public static IQueryable<Product> Search(this IQueryable<Product> products, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return products;
        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
        return products.Where(product => product.Name.ToLower().Contains(lowerCaseSearchTerm));
    }
}
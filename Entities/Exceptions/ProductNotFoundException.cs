namespace Entities.Exceptions;

public sealed class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(Guid productId) : base($"Product with id: {productId} doesn't exist in the database.") { }
}
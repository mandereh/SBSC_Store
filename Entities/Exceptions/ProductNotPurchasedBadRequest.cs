namespace Entities.Exceptions;

public class ProductNotPurchasedBadRequest : BadRequestException
{
    public ProductNotPurchasedBadRequest(string message) : base(message)
    {
    }
}
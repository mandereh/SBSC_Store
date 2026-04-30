namespace Entities.Exceptions;

public class EmptyCartBadRequest : BadRequestException
{
    public EmptyCartBadRequest(string message) : base(message) { }
}
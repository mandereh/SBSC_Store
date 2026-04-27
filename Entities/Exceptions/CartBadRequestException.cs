namespace Entities.Exceptions;

public class CartBadRequestException : BadRequestException
{
    public CartBadRequestException(string message) : base($"{message}"){}
}
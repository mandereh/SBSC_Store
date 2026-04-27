namespace Entities.Exceptions;

public class CartNotFoundException : NotFoundException
{
    public CartNotFoundException(string message) : base( $"{message}") { }
}
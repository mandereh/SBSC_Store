namespace Entities.Exceptions;

public class OrderStatusBadRequestException : BadRequestException
{
    public OrderStatusBadRequestException(string message) : base(message){}
}
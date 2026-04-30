namespace Entities.Exceptions;

public class PaymentFailedBadRequest : BadRequestException
{
    public PaymentFailedBadRequest(string message) : base(message) { }
}
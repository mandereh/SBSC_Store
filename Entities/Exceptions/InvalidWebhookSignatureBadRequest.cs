namespace Entities.Exceptions;

public class InvalidWebhookSignatureBadRequest : BadRequestException
{
    public InvalidWebhookSignatureBadRequest() : base("Invalid webhook signature.") { }
}
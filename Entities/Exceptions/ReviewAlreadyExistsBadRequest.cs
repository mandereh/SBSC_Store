namespace Entities.Exceptions;

public class ReviewAlreadyExistsBadRequest : BadRequestException
{
    public ReviewAlreadyExistsBadRequest(string message) : base(message)
    {
    }
}
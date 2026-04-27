namespace Entities.Exceptions;

public class ReviewBadRequestException : BadRequestException
{
    public ReviewBadRequestException(string message) : base(message){}
}
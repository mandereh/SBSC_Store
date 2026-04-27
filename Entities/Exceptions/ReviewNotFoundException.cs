namespace Entities.Exceptions;

public class ReviewNotFoundException : NotFoundException
{
    public ReviewNotFoundException(Guid reviewId) : base($"The review with ID {reviewId} was not found."){}
}
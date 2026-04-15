namespace Entities.Exceptions;

public sealed class CategoryCollectionBadRequest : BadRequestException
{
    public CategoryCollectionBadRequest() : base("Category collection sent from a client is null"){}
}
namespace Entities.Exceptions;

public class MaxPriceRangeBadRequestException : BadRequestException
{
    public MaxPriceRangeBadRequestException() : base("max price can't be less than min price"){}
}
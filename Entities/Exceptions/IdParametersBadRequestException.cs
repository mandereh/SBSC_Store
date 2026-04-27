namespace Entities.Exceptions;

public sealed class IdParametersBadRequestException :BadRequestException
{
    public IdParametersBadRequestException(string message) : base($"{message}") { }
}
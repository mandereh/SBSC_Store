namespace Entities.Exceptions;

public class ImageBadRequestException : BadRequestException
{
    public ImageBadRequestException(string message) : base($"{message}"){}
}
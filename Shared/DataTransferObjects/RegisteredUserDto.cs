namespace Shared.DataTransferObjects;

public record RegisteredUserDto
{
    public string? Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Role { get; init; }
}


namespace Shared.DataTransferObjects;

public record ReviewDto
{
    public Guid Id { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? UserName { get; init; }
};
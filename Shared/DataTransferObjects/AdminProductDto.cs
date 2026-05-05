namespace Shared.DataTransferObjects;

public record AdminProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal Price { get; init; }
    public string? ImageUrl { get; init; }
    public string Description { get; init; } = null!;
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
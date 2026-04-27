namespace Shared.DataTransferObjects;

public record UpdateCartItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
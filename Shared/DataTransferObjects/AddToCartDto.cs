namespace Shared.DataTransferObjects;

public record AddToCartDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
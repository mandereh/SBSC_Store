namespace Shared.DataTransferObjects;

public record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string? ProductName { get; init; }
    public decimal ProductPrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
}
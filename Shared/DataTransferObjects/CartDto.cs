namespace Shared.DataTransferObjects;

public record CartDto
{
    public List<CartItemDto> Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
};
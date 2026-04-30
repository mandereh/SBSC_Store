namespace Shared.DataTransferObjects;

public record OrderDto
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = default!;
    public string? PaymentReference { get; init; }
    public string? PaymentProvider { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<OrderItemDto> OrderItems { get; init; }
}
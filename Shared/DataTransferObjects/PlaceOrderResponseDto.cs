namespace Shared.DataTransferObjects;

public record PlaceOrderResponseDto
{
    public string OrderNumber { get; init; } = default!;
    public string PaymentReference { get; init; } = default!;
    public string PaymentUrl { get; init; } = default!;
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = default!;
}
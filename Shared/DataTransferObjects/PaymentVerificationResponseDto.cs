namespace Shared.DataTransferObjects;

public record PaymentVerificationResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = default!;
    public string Status { get; set; } = default!;
    public string Reference { get; set; } = default!;
    public decimal Amount { get; set; }
}
namespace Shared.DataTransferObjects;

public record VerifyPaymentRequestDto
{
    public string Reference { get; init; } = default!;
}
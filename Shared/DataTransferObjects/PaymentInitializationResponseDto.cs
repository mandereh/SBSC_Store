namespace Service.Contracts;

public record PaymentInitializationResponseDto
{
    public string AuthorizationUrl { get; set; } = default!;
    public string AccessCode { get; set; } = default!;
    public string Reference { get; set; } = default!;
}
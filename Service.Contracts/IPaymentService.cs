using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IPaymentService
{
    /// <summary>
    /// Initializes a payment.
    /// </summary>
    /// <param name="amount"> The amount to pay. </param>
    /// <param name="email"> The email address. </param>
    /// <param name="reference"> The payment reference. </param>
    /// <param name="cancellationToken"> The cancellation token. </param>
    /// <returns> The payment initialization response DTO. (PaymentInitializationResponseDto) </returns>
    Task<PaymentInitializationResponseDto> InitializePaymentAsync(decimal amount, string email, string reference, CancellationToken cancellationToken = default);
    /// <summary>
    /// Verifies a payment.
    /// </summary>
    /// <param name="reference"> The payment reference. </param>
    /// <param name="cancellationToken"> The cancellation token. </param>
    /// <returns> The payment verification response DTO.(PaymentVerificationResponseDto) </returns>
    Task<PaymentVerificationResponseDto> VerifyPaymentAsync(string reference, CancellationToken cancellationToken = default);
    /// <summary>
    /// Verifies the webhook signature.
    /// </summary>
    /// <param name="rawBody"> The raw request body. </param>
    /// <param name="signature"> The request signature. </param>
    /// <returns> True if the signature is valid, false otherwise. </returns>
    bool VerifyWebhookSignature(string rawBody, string signature);
    /// <summary>
    /// Extracts the webhook reference.
    /// </summary>
    /// <param name="rawBody"> The raw request body. </param>
    /// <returns> The webhook reference. </returns>
    string? ExtractWebhookReference(string rawBody);
}
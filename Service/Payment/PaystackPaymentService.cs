using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Contracts;
using Microsoft.Extensions.Options;
using SBSC_Store.Configurations;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class PaystackPaymentService : IPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PaystackSettings _paystackSettings;
    private readonly ILoggerManager _logger;
    private const string BaseUrl = "https://api.paystack.co";

    public PaystackPaymentService(IHttpClientFactory httpClientFactory, IOptions<PaystackSettings> options, ILoggerManager logger)
    {
        _httpClientFactory = httpClientFactory;
        _paystackSettings = options.Value;
        _logger = logger;
    }

    public async Task<PaymentInitializationResponseDto> InitializePaymentAsync(decimal amount, string email, string reference, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _paystackSettings.SecretKey);

        var payload = new
        {
            email = email,
            amount = (int)(amount * 100),
            reference = reference,
            callback_url = _paystackSettings.CallbackUrl
        };

        var json = JsonSerializer.Serialize(payload);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync($"{BaseUrl}/transaction/initialize", stringContent, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Paystack initialization failed: {content}");
            throw new Exception($"Paystack initialization failed: {content}");
        }

        var result = JsonSerializer.Deserialize<PaystackInitializeResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result == null || !result.Status)
        {
            _logger.LogError($"Paystack initialization returned false: {result?.Message}");
            throw new Exception($"Paystack initialization failed: {result?.Message}");
        }

        return new PaymentInitializationResponseDto
        {
            AuthorizationUrl = result.Data.AuthorizationUrl,
            AccessCode = result.Data.AccessCode,
            Reference = result.Data.Reference
        };
    }

    public async Task<PaymentVerificationResponseDto> VerifyPaymentAsync(string reference, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _paystackSettings.SecretKey);

        var response = await httpClient.GetAsync($"{BaseUrl}/transaction/verify/{reference}", cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Paystack verification failed: {content}");
            return new PaymentVerificationResponseDto { Success = false, Message = $"Verification failed: {content}" };
        }

        var result = JsonSerializer.Deserialize<PaystackVerifyResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result?.Data == null)
        {
            return new PaymentVerificationResponseDto { Success = false, Message = "Invalid response from Paystack" };
        }

        return new PaymentVerificationResponseDto
        {
            Success = result.Data.Status == "success",
            Message = result.Message,
            Reference = result.Data.Reference,
            Amount = result.Data.Amount / 100m
        };
    }

    public bool VerifyWebhookSignature(string rawBody, string signature)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(_paystackSettings.SecretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody));
        var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return computed == signature?.ToLower();
    }

    public string? ExtractWebhookReference(string rawBody)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<PaystackWebhookPayload>(rawBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return payload?.Data?.Reference;
        }
        catch
        {
            return null;
        }
    }
    
    private class PaystackInitializeResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = default!;
        public PaystackInitializeData Data { get; set; } = default!;
    }

    private class PaystackInitializeData
    {
        [JsonPropertyName("authorization_url")]
        public string AuthorizationUrl { get; set; } = default!;
        [JsonPropertyName("access_code")]
        public string AccessCode { get; set; } = default!;
        public string Reference { get; set; } = default!;
    }

    private class PaystackVerifyResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = default!;
        public PaystackVerifyData Data { get; set; } = default!;
    }

    private class PaystackVerifyData
    {
        public string Status { get; set; } = default!;
        public string Reference { get; set; } = default!;
        public int Amount { get; set; }
    }
    
    // Add this to the bottom of the file with your other private classes
    private class PaystackWebhookPayload
    {
        public string Event { get; set; } = default!;
        public PaystackWebhookData Data { get; set; } = default!;
    }

    private class PaystackWebhookData
    {
        public string Status { get; set; } = default!;
        public string Reference { get; set; } = default!;
    }
}
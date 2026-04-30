namespace SBSC_Store.Configurations;

public class PaystackSettings
{
    public string SecretKey { get; set; } = default!;
    public string PublicKey { get; set; } = default!;
    public string CallbackUrl { get; set; } = default!;
    public string BaseUrl { get; set; } = "https://api.paystack.co";
}
using Entities.Enums;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts;

namespace Service;

public class PaymentServiceFactory : IPaymentServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IPaymentService Create(PaymentServiceType type)
    {
        return type switch
        {
            PaymentServiceType.Paystack => _serviceProvider.GetRequiredService<PaystackPaymentService>(),
            _ => throw new ArgumentException("Invalid payment service type")
        };
    }
}
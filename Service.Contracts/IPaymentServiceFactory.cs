using Entities.Enums;

namespace Service.Contracts;

public interface IPaymentServiceFactory
{
    /// <summary>
    /// Creates an instance of the specified payment service type.
    /// </summary>
    /// <param name="type"> The payment service type. </param>
    /// <returns> The payment service instance. </returns>
    IPaymentService Create(PaymentServiceType type);
}
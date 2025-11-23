using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Interfaces
{
    public interface IPaymentGatewayService
    {
        Task<string> InitiatePaymentAsync(
            Guid? orderId,
            Money amount,
            string customerEmail,
            CancellationToken cancellationToken);

        Task<PaymentStatus> VerifyPaymentAsync(string transactionReference, CancellationToken cancellationToken);
    }
}

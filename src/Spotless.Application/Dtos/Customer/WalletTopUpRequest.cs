using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Customer
{
    public record WalletTopUpRequest
    {
        public decimal AmountValue { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
    }
}

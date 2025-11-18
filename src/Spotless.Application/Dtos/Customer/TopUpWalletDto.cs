using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Customer
{
    public record TopUpWalletRequest
    {
        public decimal AmountValue { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
    }
}
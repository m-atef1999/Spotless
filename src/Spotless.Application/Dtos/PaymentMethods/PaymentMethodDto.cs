using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.PaymentMethods
{
    public record PaymentMethodDto
    {
        public Guid Id { get; init; }
        public PaymentMethodType Type { get; init; }
        public string Last4Digits { get; init; } = string.Empty;
        public string CardholderName { get; init; } = string.Empty;
        public DateTime ExpiryDate { get; init; }
        public bool IsDefault { get; init; }
    }

    public record AddPaymentMethodDto
    {
        public PaymentMethodType Type { get; init; }
        public string Last4Digits { get; init; } = string.Empty;
        public string CardholderName { get; init; } = string.Empty;
        public DateTime ExpiryDate { get; init; }
        public bool IsDefault { get; init; }
    }
}

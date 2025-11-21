using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Payment
{
    public record InitiatePaymentDto
    {
        public Guid OrderId { get; init; }
        public PaymentMethod PaymentMethod { get; init; }
        public string ReturnUrl { get; init; } = string.Empty;
    }

    public record InitiatePaymentResponseDto
    {
        public Guid PaymentId { get; init; }
        public string PaymentUrl { get; init; } = string.Empty;
        public string TransactionReference { get; init; } = string.Empty;
    }
}

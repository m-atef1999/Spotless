using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Payment
{
    public record PaymentDto(
        Guid Id,
        Guid OrderId,
        decimal Amount,
        string Currency,
        DateTime PaymentDate,
        PaymentMethod Method,
        PaymentStatus Status
    );
}

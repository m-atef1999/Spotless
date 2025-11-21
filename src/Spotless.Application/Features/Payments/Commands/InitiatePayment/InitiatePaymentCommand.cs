using MediatR;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Payments.Commands.InitiatePayment
{
    public record InitiatePaymentCommand(
        Guid OrderId,
        PaymentMethod PaymentMethod,
        string ReturnUrl) : IRequest<InitiatePaymentResult>;

    public record InitiatePaymentResult(
        Guid PaymentId,
        string PaymentUrl,
        string TransactionReference);
}

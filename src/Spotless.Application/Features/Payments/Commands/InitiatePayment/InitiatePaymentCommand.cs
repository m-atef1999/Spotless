using MediatR;

namespace Spotless.Application.Features.Payments.Commands.InitiatePayment
{
    public record InitiatePaymentCommand(
        Guid OrderId,
        Guid CustomerId) : IRequest<string>;
}

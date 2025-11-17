using MediatR;

namespace Spotless.Application.Features.Payments
{
    public record InitiatePaymentCommand(
        Guid OrderId,
        Guid CustomerId) : IRequest<string>;
}

using MediatR;

namespace Spotless.Application.Features.Payments
{
    public record ProcessWebhookCommand(
        string PaymentReference) : IRequest<Unit>;
}

using MediatR;

namespace Spotless.Application.Features.Payments.Commands.ProcessWebhook
{
    public record ProcessWebhookCommand(
        string PaymentReference) : IRequest<Unit>;
}

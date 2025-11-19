using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Payments.Commands.ProcessWebhook
{
    public record ProcessWebhookCommand(
        string HmacSignature,
        PaymobProcessedCallbackData CallbackData) : IRequest<Unit>;
}

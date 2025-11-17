using MediatR;

namespace Spotless.Application.Features.Authentication
{
    public record ForgotPasswordCommand(string Email) : IRequest<bool>;
}
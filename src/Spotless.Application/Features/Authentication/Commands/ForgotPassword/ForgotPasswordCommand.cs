using MediatR;

namespace Spotless.Application.Features.Authentication.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<bool>;
}

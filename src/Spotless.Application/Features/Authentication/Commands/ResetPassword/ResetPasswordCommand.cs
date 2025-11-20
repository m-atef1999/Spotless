using MediatR;

namespace Spotless.Application.Features.Authentication.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string UserId,
        string Token,
        string NewPassword
    ) : IRequest<bool>;
}

using MediatR;

namespace Spotless.Application.Features.Authentication
{
    public record ResetPasswordCommand(
        string UserId,
        string Token,
        string NewPassword
    ) : IRequest<bool>;
}
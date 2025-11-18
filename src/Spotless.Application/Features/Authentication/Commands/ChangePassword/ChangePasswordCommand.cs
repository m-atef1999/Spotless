using MediatR;

namespace Spotless.Application.Features.Authentication.Commands.ChangePassword
{
    public record ChangePasswordCommand(
        Guid UserId,
        string CurrentPassword,
        string NewPassword
    ) : IRequest<bool>;
}
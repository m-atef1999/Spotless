using MediatR;

namespace Spotless.Application.Features.Authentication
{
    public record ChangePasswordCommand(
        Guid UserId,
        string CurrentPassword,
        string NewPassword
    ) : IRequest<bool>;
}
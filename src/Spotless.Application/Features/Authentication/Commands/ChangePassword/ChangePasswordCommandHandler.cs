using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler(IAuthService authService) : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IAuthService _authService = authService;

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {

            return await _authService.ChangePasswordAsync(
                request.UserId,
                request.CurrentPassword,
                request.NewPassword
            );
        }
    }
}
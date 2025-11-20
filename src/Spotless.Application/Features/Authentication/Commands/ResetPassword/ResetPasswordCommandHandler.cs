using MediatR;
using Spotless.Application.Interfaces;
namespace Spotless.Application.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler(IAuthService authService) : IRequestHandler<ResetPasswordCommand, bool>
    {


        private readonly IAuthService _authService = authService;

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {

            return await _authService.ResetPasswordAsync(request.UserId, request.Token, request.NewPassword);
        }
    }
}

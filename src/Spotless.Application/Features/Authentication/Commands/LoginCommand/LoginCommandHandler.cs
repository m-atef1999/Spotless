using MediatR;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication.Commands.LoginCommand
{
    public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IAuthService _authService = authService;

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var loginRequest = new LoginRequest(request.Email, request.Password);

            return await _authService.LoginAsync(loginRequest);
        }
    }
}

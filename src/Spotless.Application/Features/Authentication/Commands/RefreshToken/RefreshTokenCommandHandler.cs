using MediatR;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(IAuthService authService) : IRequestHandler<RefreshTokenCommand, AuthResult>
    {
        private readonly IAuthService _authService = authService;

        public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(request.RefreshToken);
        }
    }
}
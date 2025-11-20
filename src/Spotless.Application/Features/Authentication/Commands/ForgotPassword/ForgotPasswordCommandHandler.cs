using MediatR;
using Microsoft.Extensions.Configuration;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler(
        IAuthService authService,
        IConfiguration configuration) : IRequestHandler<ForgotPasswordCommand, bool>
    {

        private readonly IAuthService _authService = authService;
        private readonly IConfiguration _configuration = configuration;

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var clientBaseUrl = _configuration["ClientSettings:BaseUrl"];

            return await _authService.ForgotPasswordAsync(request.Email, clientBaseUrl!);
        }
    }
}
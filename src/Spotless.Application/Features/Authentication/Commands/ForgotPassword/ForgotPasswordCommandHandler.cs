using MediatR;
using Microsoft.Extensions.Configuration;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {

        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public ForgotPasswordCommandHandler(
            IAuthService authService,
            IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var clientBaseUrl = _configuration["ClientSettings:BaseUrl"];

            return await _authService.ForgotPasswordAsync(request.Email, clientBaseUrl!);
        }
    }
}
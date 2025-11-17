using MediatR;
using Microsoft.Extensions.Configuration;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication
{
    public class SendVerificationEmailCommandHandler : IRequestHandler<SendVerificationEmailCommand, bool>
    {


        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public SendVerificationEmailCommandHandler(
            IAuthService authService,
            IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        public async Task<bool> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
        {

            var clientBaseUrl = _configuration["ClientSettings:BaseUrl"];

            return await _authService.SendVerificationEmailAsync(request.UserId, clientBaseUrl!);
        }
    }
}
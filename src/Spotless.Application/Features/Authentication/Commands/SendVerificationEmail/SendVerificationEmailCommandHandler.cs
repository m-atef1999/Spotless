using MediatR;
using Microsoft.Extensions.Configuration;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication.Commands.SendVerificationEmail
{
    public class SendVerificationEmailCommandHandler(
        IAuthService authService,
        IConfiguration configuration) : IRequestHandler<SendVerificationEmailCommand, bool>
    {


        private readonly IAuthService _authService = authService;
        private readonly IConfiguration _configuration = configuration;

        public async Task<bool> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
        {

            var clientBaseUrl = _configuration["ClientSettings:BaseUrl"];

            return await _authService.SendVerificationEmailAsync(request.UserId, clientBaseUrl!);
        }
    }
}
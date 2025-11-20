using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication.Commands.SendOtp
{
    public class SendOtpCommandHandler(IAuthService authService) : IRequestHandler<SendOtpCommand, bool>
    {
        private readonly IAuthService _authService = authService;

        public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {

            return await _authService.SendPhoneVerificationOtpAsync(request.PhoneNumber);
        }
    }
}
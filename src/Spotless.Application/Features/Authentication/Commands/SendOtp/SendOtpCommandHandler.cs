using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication
{
    public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, bool>
    {
        private readonly IAuthService _authService;

        public SendOtpCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        {

            return await _authService.SendPhoneVerificationOtpAsync(request.PhoneNumber);
        }
    }
}
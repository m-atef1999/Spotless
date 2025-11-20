using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler(IAuthService authService) : IRequestHandler<VerifyOtpCommand, bool>
    {
        private readonly IAuthService _authService = authService;

        public async Task<bool> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            return await _authService.VerifyPhoneOtpAsync(request.PhoneNumber, request.Code);
        }
    }
}
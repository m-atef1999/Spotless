using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly IAuthService _authService;

        public ConfirmEmailCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {

            return await _authService.ConfirmEmailAsync(request.UserId, request.Token);
        }
    }
}
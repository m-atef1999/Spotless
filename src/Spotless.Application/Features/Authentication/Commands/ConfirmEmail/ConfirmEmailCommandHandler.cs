using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler(IAuthService authService) : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly IAuthService _authService = authService;

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {

            return await _authService.ConfirmEmailAsync(request.UserId, request.Token);
        }
    }
}

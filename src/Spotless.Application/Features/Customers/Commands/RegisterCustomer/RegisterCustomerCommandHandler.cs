using MediatR;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Customers
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, AuthResult>
    {
        private readonly IAuthService _authService;

        public RegisterCustomerCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResult> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {

            var registerRequest = new RegisterRequest(
                request.Email,
                request.Password,
                request.Name,
                request.Phone,
                request.Type,
                request.Street,
                request.City,
                request.Country,
                request.ZipCode
            );


            var authResult = await _authService.RegisterAsync(registerRequest, "Customer");

            return authResult;
        }
    }
}
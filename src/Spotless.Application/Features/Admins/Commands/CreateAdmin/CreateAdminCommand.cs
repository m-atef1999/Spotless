using MediatR;
using Spotless.Application.Dtos.Authentication;

namespace Spotless.Application.Features.Admins.Commands.CreateAdmin
{
    public class CreateAdminCommand : IRequest<AuthResult>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin"; // Admin or SuperAdmin
    }
}

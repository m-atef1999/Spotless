using Spotless.Application.Dtos.Authentication;

namespace Spotless.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request, string role);
        Task<AuthResult> LoginAsync(LoginRequest request);
    }
}

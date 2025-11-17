using Spotless.Application.Dtos.Authentication;

namespace Spotless.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request, string role);
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);

        Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);

        Task<bool> SendVerificationEmailAsync(Guid userId, string clientBaseUrl);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<bool> ForgotPasswordAsync(string email, string clientBaseUrl);
    }
}

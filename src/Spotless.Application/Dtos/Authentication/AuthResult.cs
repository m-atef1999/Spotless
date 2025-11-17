namespace Spotless.Application.Dtos.Authentication
{
    public record AuthResult(
        Guid UserId,
        string Email,
        string AccessToken,
        DateTime AccessTokenExpiration,
        string RefreshToken,
        DateTime RefreshTokenExpiration
        );
}

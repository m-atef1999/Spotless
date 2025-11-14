namespace Spotless.Application.Dtos.Authentication
{
    public record AuthResult(
        Guid UserId,
        string Email,
        string Token,
        DateTime ExpiresOn);
}

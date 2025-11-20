namespace Spotless.Application.Dtos.Authentication
{
    public record LoginRequest(
        string Email,
        string Password);
}

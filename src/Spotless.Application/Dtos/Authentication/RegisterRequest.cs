namespace Spotless.Application.Dtos.Authentication
{
    public record RegisterRequest(
        string Name,
        string Email,
        string Password,
        string? Phone);
}

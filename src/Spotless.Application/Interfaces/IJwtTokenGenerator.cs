namespace Spotless.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(IAuthUser user, string role);
    }
}

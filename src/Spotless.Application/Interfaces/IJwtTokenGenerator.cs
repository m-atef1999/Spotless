using Spotless.Application.Dtos.Authentication;

namespace Spotless.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {

        string GenerateToken(IAuthUser user, string role);


        string GenerateRefreshToken();


        AuthResult GenerateAuthResult(IAuthUser user, string role, string refreshToken);

    }
}
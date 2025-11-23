using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Spotless.Application.Configurations;
using Spotless.Application.Dtos.Authentication;
using Spotless.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Spotless.Infrastructure.Services
{
    public class JwtTokenGenerator(IOptions<JwtSettings> jwtSettings) : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public string GenerateToken(IAuthUser user, string role)
        {
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Role, role)
            };

            if (user.CustomerId.HasValue)
            {
                claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public AuthResult GenerateAuthResult(IAuthUser user, string role, string refreshToken)
        {
            var accessToken = GenerateToken(user, role);

            return new AuthResult(
                UserId: user.Id,
                Email: user.Email!,
                AccessToken: accessToken,
                AccessTokenExpiration: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                RefreshToken: refreshToken,
                RefreshTokenExpiration: DateTime.UtcNow.AddDays(_jwtSettings.RefreshExpiryDays),
                Role: role
            );
        }

    }
}

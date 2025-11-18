using MediatR;
using Spotless.Application.Dtos.Authentication;

namespace Spotless.Application.Features.Authentication.Commands.RefreshToken
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;
}
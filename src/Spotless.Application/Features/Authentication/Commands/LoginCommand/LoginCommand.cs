using MediatR;
using Spotless.Application.Dtos.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Authentication.Commands.LoginCommand
{
    public record LoginCommand(
        [EmailAddress] string Email,
        [Required] string Password
    ) : IRequest<AuthResult>;
}
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Authentication.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(
        [Required] string UserId,
        [Required] string Token
    ) : IRequest<bool>;
}

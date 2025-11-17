using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Authentication
{
    public record ConfirmEmailCommand(
        [Required] string UserId,
        [Required] string Token
    ) : IRequest<bool>;
}
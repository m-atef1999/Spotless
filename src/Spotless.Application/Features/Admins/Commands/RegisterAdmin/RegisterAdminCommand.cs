using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Admins.Commands.RegisterAdmin
{
    public record RegisterAdminCommand(
        [EmailAddress] string Email,
        [Required] string Password,
        [Required] string Name,
        string? Phone
    ) : IRequest<Guid>;
}

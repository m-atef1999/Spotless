using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Authentication
{
    public record SendOtpCommand(
        [Required] string PhoneNumber
    ) : IRequest<bool>;
}
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Authentication.Commands.SendOtp
{
    public record SendOtpCommand(
        [Required] string PhoneNumber
    ) : IRequest<bool>;
}

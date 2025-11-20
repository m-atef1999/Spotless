using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Authentication.Commands.VerifyOtp
{
    public record VerifyOtpCommand(
        [Required] string PhoneNumber,
        [Required] string Code
    ) : IRequest<bool>;
}

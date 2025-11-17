using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Spotless.Application.Features.Authentication
{
    public record VerifyOtpCommand(
        [Required] string PhoneNumber,
        [Required] string Code
    ) : IRequest<bool>;
}
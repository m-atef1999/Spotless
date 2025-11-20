using MediatR;

namespace Spotless.Application.Features.Authentication.Commands.SendVerificationEmail
{
    public record SendVerificationEmailCommand() : IRequest<bool>
    {
        public Guid UserId { get; init; }
    }
}

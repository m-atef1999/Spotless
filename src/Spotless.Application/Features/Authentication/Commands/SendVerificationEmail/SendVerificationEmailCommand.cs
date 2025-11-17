using MediatR;

namespace Spotless.Application.Features.Authentication
{
    public record SendVerificationEmailCommand() : IRequest<bool>
    {
        public Guid UserId { get; init; }
    }
}
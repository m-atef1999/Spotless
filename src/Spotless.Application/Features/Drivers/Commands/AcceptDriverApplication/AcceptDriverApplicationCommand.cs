using MediatR;

namespace Spotless.Application.Features.Drivers.Commands.AcceptDriverApplication
{
    public record AcceptDriverApplicationCommand(Guid ApplicationId, Guid AdminId) : IRequest<Unit>;
}
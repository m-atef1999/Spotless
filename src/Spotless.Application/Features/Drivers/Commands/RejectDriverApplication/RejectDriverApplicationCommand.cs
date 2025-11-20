using MediatR;

namespace Spotless.Application.Features.Drivers.Commands.RejectDriverApplication
{
    public record RejectDriverApplicationCommand(Guid ApplicationId, Guid AdminId) : IRequest<Unit>;
}

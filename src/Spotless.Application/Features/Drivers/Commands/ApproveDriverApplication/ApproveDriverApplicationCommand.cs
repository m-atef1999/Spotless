using MediatR;

namespace Spotless.Application.Features.Drivers.Commands.ApproveDriverApplication
{

    public record ApproveDriverApplicationCommand(
        Guid ApplicationId,
        Guid AdminId
    ) : IRequest<Guid>;
}

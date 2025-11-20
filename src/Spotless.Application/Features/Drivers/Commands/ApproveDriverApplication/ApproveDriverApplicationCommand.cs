using MediatR;

namespace Spotless.Application.Features.Drivers.Commands.ApproveDriverApplication
{

    public record ApproveDriverApplicationCommand(
        Guid ApplicationId,
        string Password,
        Guid AdminId
    ) : IRequest<Guid>;
}

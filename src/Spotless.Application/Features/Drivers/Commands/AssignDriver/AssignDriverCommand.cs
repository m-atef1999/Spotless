using MediatR;

namespace Spotless.Application.Features.Drivers.Commands.AssignDriver
{

    public record AssignDriverCommand(
        Guid OrderId,
        Guid DriverId,
        Guid AdminId
    ) : IRequest<Unit>;
}

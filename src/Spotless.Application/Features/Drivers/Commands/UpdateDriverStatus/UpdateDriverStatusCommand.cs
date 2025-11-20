using MediatR;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverStatus
{
    public record UpdateDriverStatusCommand(Guid DriverId, string Status) : IRequest<Unit>;
}

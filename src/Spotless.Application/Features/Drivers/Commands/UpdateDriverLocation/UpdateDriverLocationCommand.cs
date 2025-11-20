using MediatR;
using Spotless.Application.Dtos;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverLocation
{
    public record UpdateDriverLocationCommand(Guid DriverId, LocationDto Location) : IRequest<Unit>;
}

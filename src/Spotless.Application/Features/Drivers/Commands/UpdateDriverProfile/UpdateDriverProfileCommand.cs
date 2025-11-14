using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile
{
    public record UpdateDriverProfileCommand(
        UpdateDriverProfileDto Dto,
        Guid DriverId) : IRequest<Unit>;
}

using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile
{

    public class UpdateDriverProfileCommand(Guid driverId, DriverUpdateRequest dto) : IRequest<Unit>
    {
        public Guid DriverId { get; set; } = driverId;

        public DriverUpdateRequest Dto { get; set; } = dto;
    }
}

using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile
{

    public class UpdateDriverProfileCommand(Guid driverId, UpdateDriverProfileDto dto) : IRequest<Unit>
    {
        public Guid DriverId { get; set; } = driverId;

        public UpdateDriverProfileDto Dto { get; set; } = dto;
    }
}
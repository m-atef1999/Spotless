using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile
{

    public class UpdateDriverProfileCommand : IRequest<Unit>
    {
        public Guid DriverId { get; set; }

        public UpdateDriverProfileDto Dto { get; set; } = null!;

        public UpdateDriverProfileCommand(Guid driverId, UpdateDriverProfileDto dto)
        {
            DriverId = driverId;
            Dto = dto;
        }
    }
}
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Drivers.Queries.GetDriverProfile
{

    public class GetDriverProfileQuery(Guid driverId) : IQuery<DriverDto>
    {
        public Guid DriverId { get; init; } = driverId;
    }
}
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Drivers.Queries.GetDriverProfile
{

    public class GetDriverProfileQuery : IQuery<DriverDto>
    {
        public Guid DriverId { get; init; }

        public GetDriverProfileQuery(Guid driverId)
        {
            DriverId = driverId;
        }
    }
}
using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Queries.GetDriverProfile
{

    public class GetDriverProfileQuery : IRequest<DriverDto>
    {
        public Guid DriverId { get; init; }

        public GetDriverProfileQuery(Guid driverId)
        {
            DriverId = driverId;
        }
    }
}
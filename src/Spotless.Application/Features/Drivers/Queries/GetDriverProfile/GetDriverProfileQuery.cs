using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Queries.GetDriverProfile
{
    public record GetDriverProfileQuery(Guid DriverId) : IRequest<DriverProfileDto>;
}

using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers
{
    public record GetDriverProfileQuery(Guid DriverId) : IRequest<DriverProfileDto>;
}

using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Queries.GetDriverEarnings
{
    public record GetDriverEarningsQuery(Guid DriverId) : IRequest<DriverEarningsDto>;
}

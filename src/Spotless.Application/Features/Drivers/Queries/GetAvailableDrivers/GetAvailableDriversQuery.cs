using MediatR;
using Spotless.Application.Dtos.Driver;

namespace Spotless.Application.Features.Drivers.Queries.GetAvailableDrivers
{

    public class GetAvailableDriversQuery : IRequest<List<DriverDto>>
    {

    }
}
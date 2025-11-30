using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetOrderApplications
{
    public record GetOrderApplicationsQuery(Guid OrderId) : IRequest<List<OrderApplicationDto>>;
}

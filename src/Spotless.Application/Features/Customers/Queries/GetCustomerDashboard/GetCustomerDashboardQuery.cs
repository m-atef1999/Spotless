using MediatR;
using Spotless.Application.Dtos.Customer;

namespace Spotless.Application.Features.Customers.Queries.GetCustomerDashboard
{
    public record GetCustomerDashboardQuery(Guid CustomerId) : IRequest<CustomerDashboardDto>;
}


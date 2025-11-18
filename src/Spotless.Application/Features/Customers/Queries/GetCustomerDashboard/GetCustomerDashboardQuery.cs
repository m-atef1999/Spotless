using Spotless.Application.Dtos.Customer;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Customers.Queries.GetCustomerDashboard
{
    public record GetCustomerDashboardQuery(
        Guid CustomerId,
        int PageNumber = 1,
        int PageSize = 50
    ) : IQuery<CustomerDashboardDto>;
}

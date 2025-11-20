using Spotless.Application.Dtos.Customer;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Customers.Queries.GetCustomerProfile
{
    public record GetCustomerProfileQuery(Guid CustomerId) : IQuery<CustomerDto>;
}

using MediatR;
using Spotless.Application.Dtos.Customer;
namespace Spotless.Application.Features.Customers.Queries.GetCustomerProfile
{
    public record GetCustomerProfileQuery(Guid CustomerId) : IRequest<CustomerDto>;
}

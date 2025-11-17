using MediatR;
using Spotless.Application.Dtos.Customer;
namespace Spotless.Application.Features.Customers
{
    public record GetCustomerProfileQuery(Guid CustomerId) : IRequest<CustomerDto>;
}

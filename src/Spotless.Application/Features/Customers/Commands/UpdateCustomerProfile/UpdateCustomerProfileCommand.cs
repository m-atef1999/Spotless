using MediatR;
using Spotless.Application.Dtos.Customer;

namespace Spotless.Application.Features.Customers
{
    public record UpdateCustomerProfileCommand(
        UpdateCustomerDto Dto,
        Guid UserId) : IRequest<Unit>;
}

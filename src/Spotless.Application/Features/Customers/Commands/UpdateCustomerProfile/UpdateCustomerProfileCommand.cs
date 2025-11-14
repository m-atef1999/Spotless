using MediatR;
using Spotless.Application.Dtos.Customer;

namespace Spotless.Application.Features.Customers.Commands.UpdateCustomerProfile
{
    public record UpdateCustomerProfileCommand(
        UpdateCustomerDto Dto,
        Guid UserId) : IRequest<Unit>;
}

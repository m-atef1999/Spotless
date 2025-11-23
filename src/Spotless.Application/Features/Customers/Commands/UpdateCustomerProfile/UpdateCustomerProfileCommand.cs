using MediatR;
using Spotless.Application.Dtos.Customer;

namespace Spotless.Application.Features.Customers.Commands.UpdateCustomerProfile
{
    public record UpdateCustomerProfileCommand(
        CustomerUpdateRequest Dto,
        Guid UserId,
        Guid IdentityUserId) : IRequest<Unit>;
}

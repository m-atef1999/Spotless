using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Dtos.Customer
{
    public record UpdateCustomerDto(
        string Name,
        string? Phone,
        Address Address);
}

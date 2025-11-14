using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Dtos.Customer
{
    public record CustomerDto(
        Guid Id,
        string Name,
        string? Phone,
        string Email,
        Address Address,
        decimal WalletBalance,
        CustomerType Type);
}

using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Customer
{
    public record CustomerDto(
        Guid Id,
        string Name,
        string? Phone,
        string Email,
        string Street,
        string City,
        string Country,
        string? ZipCode,
        decimal WalletBalance,
        string WalletCurrency,
        CustomerType Type
    );
}

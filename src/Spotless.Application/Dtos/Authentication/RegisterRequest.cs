using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Authentication
{
    public record RegisterRequest(
        string Email,
        string Password,
        string Name,
        string? Phone,
        CustomerType Type,
        string Street,
        string City,
        string Country,
        string? ZipCode
    );
}

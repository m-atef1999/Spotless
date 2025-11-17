namespace Spotless.Application.Dtos.Customer
{
    public record RegisterCustomerDto(
        string Name,
        string Email,
        string Password,
        string Phone,

        string Street,
        string City,
        string Country,
        string? ZipCode
    );
}
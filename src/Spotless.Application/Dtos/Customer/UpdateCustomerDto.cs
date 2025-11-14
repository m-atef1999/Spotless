namespace Spotless.Application.Dtos.Customer
{
    public record UpdateCustomerDto(
        string Name,
        string? Phone,
        string Street,
        string City,
        string Country,
        string? ZipCode
    );
}

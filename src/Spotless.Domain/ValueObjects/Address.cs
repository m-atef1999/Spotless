using Spotless.Domain.Exceptions;

namespace Spotless.Domain.ValueObjects;
public sealed record Address
{

    public string Street { get; init; }
    public string City { get; init; }
    public string Country { get; init; }
    public string? ZipCode { get; init; }

    public Address(string street, string city, string country, string? zipCode = null)
    {

        if (string.IsNullOrWhiteSpace(street) ||
            string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(country))
            throw new DomainException("Address must have street, city, and country.");


        Street = street.Trim();
        City = city.Trim();
        Country = country.Trim();
        ZipCode = zipCode?.Trim();


    }


    private Address() : this("Unknown Street", "Unknown City", "Unknown Country") { }
}
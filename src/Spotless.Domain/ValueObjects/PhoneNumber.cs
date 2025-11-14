using Spotless.Domain.Exceptions;

namespace Spotless.Domain.ValueObjects;
public sealed record PhoneNumber
{

    public string Value { get; init; }

    public PhoneNumber(string value)
    {

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Phone number cannot be empty.");


        var digits = new string(value.Where(char.IsDigit).ToArray());


        if (digits.Length < 7)
            throw new DomainException("Phone number is too short or invalid.");


        Value = digits;

    }


    private PhoneNumber() : this("0000000000") { }


    public override string ToString() => Value;
}
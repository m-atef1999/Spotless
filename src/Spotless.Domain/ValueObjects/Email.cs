using Spotless.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Spotless.Domain.ValueObjects;
public sealed record Email
{

    public string Value { get; init; }


    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(250)
    );


    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.");

        string trimmedValue = value.Trim();


        if (!EmailRegex.IsMatch(trimmedValue))
            throw new DomainException($"The email address '{value}' is not valid.");


        Value = trimmedValue.ToLowerInvariant();
    }


    private Email() : this("placeholder@example.com") { }

}
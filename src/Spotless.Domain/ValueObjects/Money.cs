using Spotless.Domain.Exceptions;

namespace Spotless.Domain.ValueObjects;
public sealed record Money
{

    public decimal Amount { get; init; }
    public string Currency { get; init; }
    public static Money Zero => new Money(0m);

    public Money(decimal amount, string currency = "EGP")
    {

        if (amount < 0) throw new DomainException("Amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency required.");


        Amount = decimal.Round(amount, 2);
        Currency = currency.Trim().ToUpperInvariant();
    }


    public Money Add(Money other)
    {
        if (other.Currency != Currency)
            throw new DomainException($"Cannot add money with mismatched currencies: {Currency} != {other.Currency}.");


        return new Money(Amount + other.Amount, Currency);
    }


    private Money() : this(0.00m) { }


}
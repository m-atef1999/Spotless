using FluentAssertions;
using Spotless.Domain.Exceptions;
using Spotless.Domain.ValueObjects;

namespace Spotless.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Money_Creation_WithValidAmount_ShouldSucceed()
    {
        // Arrange & Act
        var money = new Money(100.50m, "EGP");

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("EGP");
    }

    [Fact]
    public void Money_Creation_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Arrange & Act
        var act = () => new Money(-50m, "EGP");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*negative*");
    }

    [Fact]
    public void Money_Add_WithSameCurrency_ShouldReturnCorrectSum()
    {
        // Arrange
        var money1 = new Money(50m, "EGP");
        var money2 = new Money(30m, "EGP");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(80m);
        result.Currency.Should().Be("EGP");
    }

    [Fact]
    public void Money_Add_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = new Money(50m, "EGP");
        var money2 = new Money(30m, "USD");

        // Act
        var act = () => money1.Add(money2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*currencies*");
    }

    [Fact]
    public void Money_Subtract_WithValidAmount_ShouldReturnCorrectDifference()
    {
        // Arrange
        var money1 = new Money(100m, "EGP");
        var money2 = new Money(30m, "EGP");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70m);
    }

    [Fact]
    public void Money_Subtract_ResultingInNegative_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = new Money(30m, "EGP");
        var money2 = new Money(50m, "EGP");

        // Act
        var act = () => money1.Subtract(money2);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Money_Multiply_ShouldReturnCorrectProduct()
    {
        // Arrange
        var money = new Money(25m, "EGP");

        // Act
        var result = money.Multiply(4);

        // Assert
        result.Amount.Should().Be(100m);
        result.Currency.Should().Be("EGP");
    }
}

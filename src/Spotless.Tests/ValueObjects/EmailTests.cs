using FluentAssertions;
using Spotless.Domain.Exceptions;
using Spotless.Domain.ValueObjects;

namespace Spotless.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Email_Creation_WithValidEmail_ShouldSucceed()
    {
        // Arrange & Act
        var email = new Email("user@example.com");

        // Assert
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void Email_Creation_WithInvalidFormat_ShouldThrowDomainException()
    {
        // Arrange & Act
        var act = () => new Email("invalid-email");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*not valid*");
    }

    [Fact]
    public void Email_Creation_WithEmptyString_ShouldThrowDomainException()
    {
        // Arrange & Act
        var act = () => new Email("");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*empty*");
    }

    [Fact]
    public void Email_Value_ShouldBeLowercased()
    {
        // Arrange & Act
        var email = new Email("USER@EXAMPLE.COM");

        // Assert
        email.Value.Should().Be("user@example.com");
    }
}

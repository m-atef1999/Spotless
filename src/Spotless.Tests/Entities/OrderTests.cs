using FluentAssertions;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

namespace Spotless.Tests.Entities;

public class OrderTests
{
    [Fact]
    public void Order_StatusTransition_FromRequestedToConfirmed_ShouldSucceed()
    {
        // Arrange
        var order = CreateValidOrder();

        // Act
        order.SetStatus(OrderStatus.Confirmed);

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Order_StatusTransition_FromRequestedToCancelled_ShouldSucceed()
    {
        // Arrange
        var order = CreateValidOrder();

        // Act
        order.SetStatus(OrderStatus.Cancelled);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Order_StatusTransition_FromDelivered_ShouldThrowInvalidOperation()
    {
        // Arrange - Get order to Delivered state
        var order = CreateValidOrder();
        order.SetStatus(OrderStatus.Delivered);

        // Act - Try to transition from Delivered (a final state)
        var act = () => order.SetStatus(OrderStatus.Confirmed);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*final state*");
    }

    [Fact]
    public void Order_AssignDriver_ShouldSetDriverId()
    {
        // Arrange
        var order = CreateValidOrder();
        var driverId = Guid.NewGuid();

        // Act
        order.AssignDriver(driverId);

        // Assert
        order.DriverId.Should().Be(driverId);
    }

    [Fact]
    public void Order_StatusTransition_FromCancelled_ShouldThrowInvalidOperation()
    {
        // Arrange - Cancel the order first
        var order = CreateValidOrder();
        order.SetStatus(OrderStatus.Cancelled);

        // Act - Try to transition from Cancelled (a final state)
        var act = () => order.SetStatus(OrderStatus.Confirmed);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*final state*");
    }

    private static Order CreateValidOrder()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();
        var timeSlotId = Guid.NewGuid();
        
        var orderItem = new OrderItem(orderId, serviceId, new Money(50m, "EGP"), 1);
        var items = new List<OrderItem> { orderItem };
        var totalPrice = new Money(50m, "EGP");
        var pickupLocation = new Location(30.0444m, 31.2357m);
        var deliveryLocation = new Location(30.0500m, 31.2400m);

        return new Order(
            customerId, 
            items, 
            totalPrice, 
            timeSlotId, 
            DateTime.UtcNow.AddDays(1),
            PaymentMethod.CashOnDelivery, 
            pickupLocation, 
            deliveryLocation,
            "123 Test Street",
            "456 Delivery Ave"
        );
    }
}

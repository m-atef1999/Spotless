using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Carts.Commands.Checkout
{
    public record CheckoutCommand(
        Guid CustomerId,
        Guid TimeSlotId,
        DateTime ScheduledDate,
        PaymentMethod PaymentMethod,
        decimal PickupLatitude,
        decimal PickupLongitude,
        decimal DeliveryLatitude,
        decimal DeliveryLongitude
    ) : IRequest<Guid>;
}

using Spotless.Application.Dtos.Order;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Mappers
{
    public static class OrderMapper
    {

        public static OrderDto ToDto(this Order order)
        {

            string serviceName = order.Service?.Name ?? "Unknown Service";

            return new OrderDto(
                Id: order.Id,
                CustomerId: order.CustomerId,
                ServiceId: order.ServiceId,
                ServiceName: serviceName,
                TotalPrice: $"{order.TotalPrice.Amount:N2} {order.TotalPrice.Currency}",
                Status: order.Status.ToString(),
                OrderDate: order.OrderDate,
                DeliveryTime: order.DeliveryTime);
        }

        public static Order ToEntity(this CreateOrderDto dto, Guid customerId, Money calculatedPrice)
        {
            return new Order(
                customerId: customerId,
                serviceId: dto.ServiceId,
                totalPrice: calculatedPrice,
                pickupTime: dto.PickupTime,
                deliveryTime: dto.DeliveryTime,
                paymentMethod: dto.PaymentMethod);
        }
    }
}

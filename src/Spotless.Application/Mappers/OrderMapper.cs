using Spotless.Application.Dtos.Order;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Mappers
{
    public static class OrderMapper
    {

        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto(
                Id: order.Id,
                CustomerId: order.CustomerId,
                ServiceId: order.ServiceId,
                DriverId: order.DriverId,
                TotalPrice: order.TotalPrice.Amount,
                Currency: order.TotalPrice.Currency,
                PickupTime: order.PickupTime,
                DeliveryTime: order.DeliveryTime,
                Status: order.Status,
                PaymentMethod: order.PaymentMethod,
                OrderDate: order.OrderDate
            );
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

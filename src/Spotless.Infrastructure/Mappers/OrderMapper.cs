using Spotless.Application.Dtos.Order;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;

namespace Spotless.Infrastructure.Mappers
{

    public class OrderMapper : IOrderMapper
    {

        private OrderItemDto MapOrderItemToDto(OrderItem item)
        {
            return new OrderItemDto(
                Id: item.Id, 
                ServiceId: item.ServiceId,
                PriceAmount: item.Price.Amount,
                PriceCurrency: item.Price.Currency,
                Quantity: item.Quantity
            );
        }



        public Order MapToEntity(CreateOrderDto dto, Guid customerId, Money totalPrice, List<Money> itemPrices)
        {
            var orderItems = dto.Items.Select((itemDto, index) =>
                new OrderItem(
                    orderId: Guid.Empty,
                    serviceId: itemDto.ServiceId,
                    price: itemPrices[index],
                    quantity: itemDto.Quantity
                )
            ).ToList();

            var pickupLocation = new Location(dto.PickupLatitude, dto.PickupLongitude);
            var deliveryLocation = new Location(dto.DeliveryLatitude, dto.DeliveryLongitude);

            return new Order(
                customerId: customerId,
                items: orderItems,
                totalPrice: totalPrice,
                timeSlotId: dto.TimeSlotId,
                scheduledDate: dto.ScheduledDate,
                paymentMethod: dto.PaymentMethod,
                pickupLocation: pickupLocation,
                deliveryLocation: deliveryLocation
            );
        }

        public OrderDto MapToDto(Order order)
        {
            if (order == null) return null!;

            return new OrderDto(
                Id: order.Id,
                CustomerId: order.CustomerId,
                DriverId: order.DriverId,
                TimeSlotId: order.TimeSlotId,
                ScheduledDate: order.ScheduledDate,

                PickupLatitude: order.PickupLocation?.Latitude ?? 0.0m,
                PickupLongitude: order.PickupLocation?.Longitude ?? 0.0m,
                DeliveryLatitude: order.DeliveryLocation?.Latitude ?? 0.0m,
                DeliveryLongitude: order.DeliveryLocation?.Longitude ?? 0.0m,


                TotalPrice: order.TotalPrice.Amount,
                Currency: order.TotalPrice.Currency,

                Status: order.Status,
                PaymentMethod: order.PaymentMethod,
                OrderDate: order.OrderDate,
                Items: order.Items.Select(MapOrderItemToDto).ToList()
            );
        }


        public IEnumerable<OrderDto> MapToDto(IEnumerable<Order> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}
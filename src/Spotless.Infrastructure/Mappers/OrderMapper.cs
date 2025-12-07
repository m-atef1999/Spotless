using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
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
                Quantity: item.Quantity,
                ServiceName: item.Service?.Name ?? "Unknown Service"
            );
        }



        public Order MapToEntity(CreateOrderDto dto, Guid customerId, Money totalPrice, List<PriceCalculationResult> itemPrices)
        {
            var orderItems = dto.Items.Select((itemDto, index) =>
                new OrderItem(
                    orderId: Guid.Empty,
                    serviceId: itemDto.ServiceId,
                    price: itemPrices[index].UnitPrice,
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
                deliveryLocation: deliveryLocation,
                pickupAddress: dto.PickupAddress,
                deliveryAddress: dto.DeliveryAddress
            );
        }

        public OrderDto MapToDto(Order order)
        {
            if (order == null) return null!;

            return new OrderDto(
                Id: order.Id,
                CustomerId: order.CustomerId,
                CustomerName: order.Customer?.Name ?? "Unknown",
                DriverId: order.DriverId,
                DriverName: order.Driver?.Name,
                TimeSlotId: order.TimeSlotId,
                StartTime: order.TimeSlot?.StartTime,
                EndTime: order.TimeSlot?.EndTime,
                ScheduledDate: order.ScheduledDate,

                PickupLatitude: order.PickupLocation?.Latitude ?? 0.0m,
                PickupLongitude: order.PickupLocation?.Longitude ?? 0.0m,
                PickupAddress: order.PickupAddress,
                DeliveryLatitude: order.DeliveryLocation?.Latitude ?? 0.0m,
                DeliveryLongitude: order.DeliveryLocation?.Longitude ?? 0.0m,
                DeliveryAddress: order.DeliveryAddress,

                TotalPrice: order.TotalPrice.Amount,
                Currency: order.TotalPrice.Currency,

                Status: order.Status,
                PaymentMethod: order.PaymentMethod,
                ServiceName: order.Items.FirstOrDefault()?.Service?.Name ?? "Unknown Service",
                CreatedAt: order.OrderDate,
                OrderDate: order.OrderDate,
                EstimatedDurationHours: order.Items.Sum(i => (i.Service?.EstimatedDurationHours ?? 0) * i.Quantity),
                Items: order.Items.Select(MapOrderItemToDto).ToList()
            );
        }


        public IEnumerable<OrderDto> MapToDto(IEnumerable<Order> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}

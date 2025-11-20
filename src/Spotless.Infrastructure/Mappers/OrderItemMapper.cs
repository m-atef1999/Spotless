using Spotless.Application.Dtos.Order;
using Spotless.Domain.Entities;

namespace Spotless.Infrastructure.Mappers
{
    public static class OrderItemMapper
    {

        public static OrderItemDto ToDto(this OrderItem item)
        {

            return new OrderItemDto(
                Id: item.Id,
                ServiceId: item.ServiceId,
                PriceAmount: item.Price.Amount,
                PriceCurrency: item.Price.Currency,
                Quantity: item.Quantity
            );
        }
    }
}

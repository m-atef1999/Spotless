using Spotless.Application.Dtos.Order;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Mappers
{
    public interface IOrderMapper
    {

        OrderDto MapToDto(Order entity);
        IEnumerable<OrderDto> MapToDto(IEnumerable<Order> entities);

        Order MapToEntity(
        CreateOrderDto dto,
        Guid customerId,
        Money totalPrice,
        List<Money> itemPrices);
    }
}

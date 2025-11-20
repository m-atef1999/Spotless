using MediatR;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(
        CreateOrderDto Dto,
        Guid CustomerId) : IRequest<OrderDto>;
}

using MediatR;
using Spotless.Application.Dtos.Order;

namespace Spotless.Application.Features.Orders
{
    public record CreateOrderCommand(
        CreateOrderDto Dto,
        Guid CustomerId) : IRequest<OrderDto>;
}

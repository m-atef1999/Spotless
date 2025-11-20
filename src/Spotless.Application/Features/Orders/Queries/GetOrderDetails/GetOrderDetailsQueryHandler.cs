using MediatR;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Orders.Queries.GetOrderDetails
{
    public class GetOrderDetailsQueryHandler(IUnitOfWork unitOfWork, IOrderMapper orderMapper) : IRequestHandler<GetOrderDetailsQuery, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOrderMapper _orderMapper = orderMapper;

        public async Task<OrderDto> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetSingleAsync(
                o => o.Id == request.OrderId,
                include: query => query
                    .Include(o => o.Customer)
                    .Include(o => o.Items)
            ) ?? throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            return _orderMapper.MapToDto(order);
        }
    }
}
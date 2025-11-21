using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.AcceptOrder
{
    public class AcceptOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IOrderMapper orderMapper) : IRequestHandler<AcceptOrderCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOrderMapper _orderMapper = orderMapper;

        public async Task<OrderDto> Handle(AcceptOrderCommand request, CancellationToken cancellationToken)
        {
            // Get the order
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId)
                ?? throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

            // Verify order is available for assignment
            if (order.Status != OrderStatus.Confirmed)
            {
                throw new InvalidOperationException(
                    $"Order cannot be accepted. Current status is {order.Status}. Only orders with 'Confirmed' status can be accepted.");
            }

            // Check if order already has a driver
            if (order.DriverId.HasValue)
            {
                throw new InvalidOperationException("This order has already been assigned to a driver.");
            }

            // Assign driver to order
            order.AssignDriver(request.DriverId);

            await _unitOfWork.CommitAsync();

            return _orderMapper.MapToDto(order);
        }
    }
}

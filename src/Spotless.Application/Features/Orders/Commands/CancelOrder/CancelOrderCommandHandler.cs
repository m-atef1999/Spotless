using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            }


            if (order.CustomerId != request.CustomerId)
            {

                throw new UnauthorizedAccessException($"Customer ID {request.CustomerId} is not authorized to cancel Order ID {request.OrderId}.");
            }


            if (order.Status == OrderStatus.PickedUp || order.Status == OrderStatus.InCleaning || order.Status == OrderStatus.OutForDelivery || order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException($"Order ID {request.OrderId} cannot be cancelled because its current status is {order.Status}.");
            }


            order.SetStatus(OrderStatus.Cancelled);


            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            }

            if (order.CustomerId != request.CustomerId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this order.");
            }

            order.UpdateDetails(
                request.Dto.ServiceId,
                request.Dto.PickupTime,
                request.Dto.DeliveryTime
            );

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

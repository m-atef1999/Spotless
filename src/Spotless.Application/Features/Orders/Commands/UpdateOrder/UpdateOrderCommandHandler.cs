using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Orders
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

            var newPickupLocation = new Location(request.Dto.PickupLatitude, request.Dto.PickupLongitude);
            var newDeliveryLocation = new Location(request.Dto.DeliveryLatitude, request.Dto.DeliveryLongitude);

            order.UpdateDetails(
                request.Dto.TimeSlotId,
                request.Dto.ScheduledDate,
                newPickupLocation,
                newDeliveryLocation
            );

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

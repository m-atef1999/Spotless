using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.ValueObjects;

namespace Spotless.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {

            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId) ?? throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            if (order.CustomerId != request.CustomerId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this order.");
            }


            Guid timeSlotId = request.Dto.TimeSlotId ?? order.TimeSlotId;
            DateTime scheduledDate = request.Dto.ScheduledDate ?? order.ScheduledDate;


            Location newPickupLocation = order.PickupLocation;
            Location newDeliveryLocation = order.DeliveryLocation;



            if (request.Dto.PickupLatitude.HasValue && request.Dto.PickupLongitude.HasValue)
            {
                newPickupLocation = new Location(
                    request.Dto.PickupLatitude.Value,
                    request.Dto.PickupLongitude.Value
                );
            }

            if (request.Dto.DeliveryLatitude.HasValue && request.Dto.DeliveryLongitude.HasValue)
            {
                newDeliveryLocation = new Location(
                    request.Dto.DeliveryLatitude.Value,
                    request.Dto.DeliveryLongitude.Value
                );
            }


            order.UpdateDetails(
                timeSlotId,
                scheduledDate,
                newPickupLocation,
                newDeliveryLocation
            );


            if (request.Dto.ServiceId.HasValue)
            {
                order.UpdateService(request.Dto.ServiceId.Value);
            }



            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
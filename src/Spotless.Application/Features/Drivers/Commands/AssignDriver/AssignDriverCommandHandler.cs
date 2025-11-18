using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Commands.AssignDriver
{
    public class AssignDriverCommandHandler : IRequestHandler<AssignDriverCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AssignDriverCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(AssignDriverCommand request, CancellationToken cancellationToken)
        {

            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            }
            if (driver == null)
            {
                throw new KeyNotFoundException($"Driver with ID {request.DriverId} not found.");
            }



            if (order.Status != OrderStatus.Confirmed || order.DriverId.HasValue)
            {
                throw new InvalidOperationException($"Order ID {request.OrderId} is not eligible for assignment. Current status: {order.Status}.");
            }

            if (driver.Status != DriverStatus.Available)
            {
                throw new InvalidOperationException($"Driver ID {request.DriverId} cannot be assigned. Current status: {driver.Status}.");
            }

            order.AssignDriver(request.DriverId);
            order.SetStatus(OrderStatus.DriverAssigned);
            driver.UpdateStatus(DriverStatus.OnRoute);

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.Drivers.UpdateAsync(driver);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
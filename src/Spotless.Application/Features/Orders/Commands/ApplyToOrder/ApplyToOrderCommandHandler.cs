using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.ApplyToOrder
{
    public class ApplyToOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ApplyToOrderCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(ApplyToOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId) ?? throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            if (order.Status != OrderStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed orders are available for driver applications.");

            if (order.DriverId.HasValue)
                throw new InvalidOperationException("Order already has an assigned driver.");

            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId) ?? throw new KeyNotFoundException($"Driver with ID {request.DriverId} not found.");
            if (driver.Status != DriverStatus.Available)
                throw new InvalidOperationException("Driver is not available to apply for orders.");

            var existing = (await _unitOfWork.OrderDriverApplications.GetAsync(a => a.OrderId == request.OrderId && a.DriverId == request.DriverId)).FirstOrDefault();
            if (existing != null)
                throw new InvalidOperationException("You have already applied to this order.");

            var application = new OrderDriverApplication(request.OrderId, request.DriverId);

            await _unitOfWork.OrderDriverApplications.AddAsync(application);
            await _unitOfWork.CommitAsync();

            return application.Id;
        }
    }
}

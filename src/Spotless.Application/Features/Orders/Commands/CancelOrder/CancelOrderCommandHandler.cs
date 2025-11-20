using MediatR;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IOptions<NotificationSettings> settings) : IRequestHandler<CancelOrderCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationService _notificationService = notificationService;
        private readonly NotificationSettings _settings = settings.Value;

        public async Task<Unit> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId) ?? throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            if (order.CustomerId != request.CustomerId)
            {

                throw new UnauthorizedAccessException($"Customer ID {request.CustomerId} is not authorized to cancel Order ID {request.OrderId}.");
            }





            order.SetStatus(OrderStatus.Cancelled);

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            if (_settings.NotifyOnOrderCancelled && _settings.EnableEmailNotifications)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
                if (customer?.Email != null)
                    await _notificationService.SendEmailNotificationAsync(customer.Email, "Order Cancelled", $"Your order #{order.Id} has been cancelled.");
            }

            return Unit.Value;
        }
    }
}

using MediatR;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.ConfirmOrder.Commands
{
    public class ConfirmOrderCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IAuthService authService, IOptions<NotificationSettings> settings) : IRequestHandler<ConfirmOrderCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAuthService _authService = authService;
        private readonly NotificationSettings _settings = settings.Value;

        public async Task<Unit> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId) ?? throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");
            order.SetStatus(OrderStatus.Confirmed);

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            if (_settings.NotifyOnOrderConfirmed)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
                
                // Email
                if (_settings.EnableEmailNotifications && customer?.Email != null)
                {
                    await _notificationService.SendEmailNotificationAsync(customer.Email, "Order Confirmed", $"Your order #{order.Id} has been confirmed.");
                }

                // Push
                var userId = await _authService.GetUserIdByCustomerIdAsync(order.CustomerId);
                if (userId != null)
                {
                    await _notificationService.SendPushNotificationAsync(userId, "Order Confirmed", $"Your order #{order.Id} has been confirmed.");
                }
            }

            return Unit.Value;
        }
    }
}

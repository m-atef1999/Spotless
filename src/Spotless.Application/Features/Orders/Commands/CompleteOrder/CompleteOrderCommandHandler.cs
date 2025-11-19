using MediatR;
using Microsoft.Extensions.Options;
using Spotless.Application.Configurations;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.CompleteOrder
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly NotificationSettings _settings;

        public CompleteOrderCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IOptions<NotificationSettings> settings)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _settings = settings.Value;
        }

        public async Task<Unit> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.OrderId} not found.");

            order.SetStatus(OrderStatus.Delivered);

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.CommitAsync();

            if (_settings.NotifyOnOrderCompleted && _settings.EnableEmailNotifications)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(order.CustomerId);
                if (customer?.Email != null)
                    await _notificationService.SendEmailNotificationAsync(customer.Email, "Order Delivered - Please Review", $"Your order #{order.Id} has been delivered. We'd love to hear your feedback!");
            }

            return Unit.Value;
        }
    }
}

using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Domain.Exceptions;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler(
        IUnitOfWork unitOfWork, 
        IOrderMapper mapper,
        INotificationService notificationService,
        IAuthService authService) : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOrderMapper _mapper = mapper;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAuthService _authService = authService;

        public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                throw new NotFoundException($"Order with ID {request.OrderId} not found.");
            }

            var oldStatus = order.Status;
            order.UpdateStatus(request.NewStatus);
            await _unitOfWork.CommitAsync();

            // Send notifications based on status change
            var orderId = order.Id.ToString().Substring(0, 8);

            // Notify Customer
            var customerUserId = await _authService.GetUserIdByCustomerIdAsync(order.CustomerId);
            if (customerUserId != null)
            {
                var (title, message) = request.NewStatus switch
                {
                    OrderStatus.PickedUp => ("Order Picked Up", $"Your order #{orderId} has been picked up by the driver."),
                    OrderStatus.InCleaning => ("Order In Cleaning", $"Your order #{orderId} is now being cleaned."),
                    OrderStatus.OutForDelivery => ("Order Out for Delivery", $"Your order #{orderId} is on its way to you!"),
                    OrderStatus.Delivered => ("Order Delivered", $"Your order #{orderId} has been delivered. Thank you for using Spotless!"),
                    _ => (null, null)
                };

                if (title != null && message != null)
                {
                    await _notificationService.SendPushNotificationAsync(customerUserId, title, message);
                }
            }

            // Notify Driver when order is delivered (completion notification)
            if (request.NewStatus == OrderStatus.Delivered && order.DriverId.HasValue)
            {
                var driverUserId = await _authService.GetUserIdByDriverIdAsync(order.DriverId.Value);
                if (driverUserId != null)
                {
                    await _notificationService.SendPushNotificationAsync(
                        driverUserId, 
                        "Order Completed! 🎉", 
                        $"Order #{orderId} has been successfully delivered. Great job!"
                    );
                }
            }

            return _mapper.MapToDto(order);
        }
    }
}

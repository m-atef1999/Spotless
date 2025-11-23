using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Services;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;
using Spotless.Domain.Entities;
using Spotless.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Spotless.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(
        IUnitOfWork unitOfWork,
        IPricingService pricingService,
        IOrderMapper orderMapper,
        IDomainEventPublisher eventPublisher,
        CachedTimeSlotService cachedTimeSlotService,
        INotificationService notificationService,
        IAuthService authService,
        ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPricingService _pricingService = pricingService;
        private readonly IOrderMapper _orderMapper = orderMapper;
        private readonly IDomainEventPublisher _eventPublisher = eventPublisher;
        private readonly CachedTimeSlotService _cachedTimeSlotService = cachedTimeSlotService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAuthService _authService = authService;
        private readonly ILogger<CreateOrderCommandHandler> _logger = logger;

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Calculate Prices
                var pricingItems = request.Dto.Items.Select(item => new PricingItemDto(
                    item.ServiceId,
                    item.ItemName,
                    item.Quantity
                )).ToList();

                var itemPrices = await _pricingService.GetItemPricesAsync(pricingItems);
                var calculatedTotalPrice = _pricingService.CalculateTotal(itemPrices);
                var calculatedItemPrices = itemPrices.Select(p => p.Price).ToList();

                // 2. Get Time Slot and Max Capacity
                var timeSlot = await _unitOfWork.Orders.GetTimeSlotByIdAsync(request.Dto.TimeSlotId)
                    ?? throw new KeyNotFoundException($"TimeSlot with ID {request.Dto.TimeSlotId} not found.");

                // 3. Create Locations
                var pickupLocation = new Location(request.Dto.PickupLatitude, request.Dto.PickupLongitude);
                var deliveryLocation = new Location(request.Dto.DeliveryLatitude, request.Dto.DeliveryLongitude);

                // 4. Create Order Items
                var orderItems = new List<OrderItem>();
                foreach (var item in request.Dto.Items)
                {
                    var itemPrice = itemPrices.First(p => p.ServiceId == item.ServiceId).Price;
                    // Pass Guid.Empty for OrderId, relying on EF Core to set it when Order is saved
                    orderItems.Add(new OrderItem(Guid.Empty, item.ServiceId, itemPrice, item.Quantity));
                }

                // 5. Create Order Entity
                var order = new Order(
                    customerId: request.CustomerId,
                    items: orderItems,
                    totalPrice: calculatedTotalPrice.Total,
                    timeSlotId: request.Dto.TimeSlotId,
                    scheduledDate: request.Dto.ScheduledDate,
                    paymentMethod: request.Dto.PaymentMethod,
                    pickupLocation: pickupLocation,
                    deliveryLocation: deliveryLocation,
                    pickupAddress: request.Dto.PickupAddress,
                    deliveryAddress: request.Dto.DeliveryAddress
                );

                // 6. Add Order with Slot Lock
                await _unitOfWork.Orders.AddOrderWithSlotLockAsync(order, request.Dto.TimeSlotId, request.Dto.ScheduledDate, timeSlot.MaxCapacity);
                
                // 7. Commit
                await _unitOfWork.CommitAsync();

                // 8. Map to DTO
                var resultDto = _orderMapper.MapToDto(order);
                
                // Populate Service Names for display
                var updatedItems = resultDto.Items.Select(i => 
                {
                    var pricingItem = pricingItems.FirstOrDefault(p => p.ServiceId == i.ServiceId);
                    return i with { ServiceName = pricingItem?.ItemName ?? "Service" };
                }).ToList();

                var finalDto = resultDto with { Items = updatedItems, ServiceName = updatedItems.FirstOrDefault()?.ServiceName ?? "Service" };

                // 9. Send Notifications
                try
                {
                    var userId = await _authService.GetUserIdByCustomerIdAsync(request.CustomerId);
                    if (userId != null)
                    {
                        await _notificationService.SendPushNotificationAsync(userId, "Order Created", $"Your order #{finalDto.Id.ToString().Substring(0, 8)} has been successfully created.");
                    }

                    // Notify Admins
                    var adminIds = await _authService.GetAdminUserIdsAsync();
                    foreach (var adminId in adminIds)
                    {
                        await _notificationService.SendPushNotificationAsync(adminId, "New Order Created", $"New order #{finalDto.Id.ToString().Substring(0, 8)} created.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send order creation notification for Customer {CustomerId}", request.CustomerId);
                }

                return finalDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);
                throw;
            }
        }
    }
}

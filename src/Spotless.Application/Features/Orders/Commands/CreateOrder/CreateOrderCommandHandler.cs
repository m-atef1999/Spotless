using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Services;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;


namespace Spotless.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(IUnitOfWork unitOfWork, IPricingService pricingService, IOrderMapper orderMapper, IDomainEventPublisher eventPublisher, CachedTimeSlotService cachedTimeSlotService) : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPricingService _pricingService = pricingService;
        private readonly IOrderMapper _orderMapper = orderMapper;
        private readonly IDomainEventPublisher _eventPublisher = eventPublisher;
        private readonly CachedTimeSlotService _cachedTimeSlotService = cachedTimeSlotService;

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var pricingItems = request.Dto.Items.Select(item => new PricingItemDto(
                item.ServiceId,
                item.ItemName,
                item.Quantity
            )).ToList();


            var itemPrices = await _pricingService.GetItemPricesAsync(pricingItems);


            var calculatedTotalPrice = _pricingService.CalculateTotal(itemPrices);

            var calculatedItemPrices = itemPrices.Select(p => p.Price).ToList();

            // Check time slot capacity to avoid overbooking
            var timeSlot = await _unitOfWork.Orders.GetTimeSlotByIdAsync(request.Dto.TimeSlotId) ?? throw new InvalidOperationException("Selected time slot does not exist.");
            var existingCount = await _unitOfWork.Orders.CountOrdersBySlotAsync(request.Dto.TimeSlotId, request.Dto.ScheduledDate.Date);
            if (existingCount >= timeSlot.MaxCapacity)
                throw new InvalidOperationException("Selected time slot is full. Please choose another time.");

            var orderEntity = _orderMapper.MapToEntity(
                dto: request.Dto,
                customerId: request.CustomerId,
                totalPrice: calculatedTotalPrice.Total,
                itemPrices: calculatedItemPrices
            );

            // Use a DB-level locked add to prevent overbooking in concurrent scenarios.
            await _unitOfWork.Orders.AddOrderWithSlotLockAsync(orderEntity, request.Dto.TimeSlotId, request.Dto.ScheduledDate.Date, timeSlot.MaxCapacity);
            // Invalidate time-slot cache so available slots reflect the new booking
            try
            {
                await _cachedTimeSlotService.InvalidateTimeSlotCacheAsync();
            }
            catch { /* cache invalidation is best-effort for prototype */ }
            
            // Publish domain event
            var orderCreatedEvent = orderEntity.CreateOrderCreatedEvent();
            await _eventPublisher.PublishAsync(orderCreatedEvent);

            return _orderMapper.MapToDto(orderEntity);
        }
    }
}
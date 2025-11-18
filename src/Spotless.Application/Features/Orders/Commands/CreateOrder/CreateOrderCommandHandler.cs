using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;


namespace Spotless.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPricingService _pricingService;
        private readonly IOrderMapper _orderMapper;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IPricingService pricingService, IOrderMapper orderMapper)
        {
            _unitOfWork = unitOfWork;
            _pricingService = pricingService;
            _orderMapper = orderMapper;
        }

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

            var orderEntity = _orderMapper.MapToEntity(
                dto: request.Dto,
                customerId: request.CustomerId,
                totalPrice: calculatedTotalPrice.Total,
                itemPrices: calculatedItemPrices
            );

            orderEntity.SetStatus(OrderStatus.Requested);

            await _unitOfWork.Orders.AddAsync(orderEntity);
            await _unitOfWork.CommitAsync();

            return _orderMapper.MapToDto(orderEntity);
        }
    }
}
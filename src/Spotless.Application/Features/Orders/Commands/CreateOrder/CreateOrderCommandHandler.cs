using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;


namespace Spotless.Application.Features.Orders
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IPricingService _pricingService;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IPricingService pricingService)
        {
            _unitOfWork = unitOfWork;
            _pricingService = pricingService;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            var itemPrices = await _pricingService.GetItemPricesAsync(request.Dto.Items);

            var calculatedTotalPrice = _pricingService.CalculateTotal(itemPrices);

            var orderEntity = request.Dto.ToEntity(
            request.CustomerId,
            calculatedTotalPrice,
            itemPrices.Select(p => p.Price).ToList()
                );


            orderEntity.SetStatus(OrderStatus.Requested);

            await _unitOfWork.Orders.AddAsync(orderEntity);


            await _unitOfWork.CommitAsync();

            return orderEntity.ToDto();
        }
    }
}

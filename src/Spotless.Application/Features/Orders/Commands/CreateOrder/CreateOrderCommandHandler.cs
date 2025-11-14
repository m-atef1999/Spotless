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

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IPricingService pricingService)
        {
            _unitOfWork = unitOfWork;
            _pricingService = pricingService;
        }

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            var calculatedPrice = await _pricingService.GetBasePriceAsync(request.Dto.ServiceId);


            var orderEntity = request.Dto.ToEntity(request.CustomerId, calculatedPrice);


            orderEntity.SetStatus(OrderStatus.Requested);

            return orderEntity.ToDto();
        }
    }
}

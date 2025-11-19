using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Orders.Queries.GetDriverOrders
{
    public class GetDriverOrdersQueryHandler : IRequestHandler<GetDriverOrdersQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderMapper _orderMapper;

        public GetDriverOrdersQueryHandler(IUnitOfWork unitOfWork, IOrderMapper orderMapper)
        {
            _unitOfWork = unitOfWork;
            _orderMapper = orderMapper;
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(GetDriverOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Orders.GetAvailableOrdersForDriverAsync(request.DriverId);

            var dtos = _orderMapper.MapToDto(orders).ToList();

            return dtos;
        }
    }
}
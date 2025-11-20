using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;
using System.Linq;

namespace Spotless.Application.Features.Orders.Queries.GetAvailableForDrivers
{
    public class GetAvailableForDriversQueryHandler(IUnitOfWork unitOfWork, IOrderMapper orderMapper) : IRequestHandler<GetAvailableForDriversQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOrderMapper _orderMapper = orderMapper;

        public async Task<IReadOnlyList<OrderDto>> Handle(GetAvailableForDriversQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.Orders.GetAsync(o => o.Status == OrderStatus.Confirmed && !o.DriverId.HasValue);

            return _orderMapper.MapToDto(orders).ToList();
        }
    }
}
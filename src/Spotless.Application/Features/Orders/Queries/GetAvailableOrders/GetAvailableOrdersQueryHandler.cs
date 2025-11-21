using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Queries.GetAvailableOrders
{
    public class GetAvailableOrdersQueryHandler(
        IUnitOfWork unitOfWork,
        IOrderMapper orderMapper) : IRequestHandler<GetAvailableOrdersQuery, PagedResponse<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOrderMapper _orderMapper = orderMapper;

        public async Task<PagedResponse<OrderDto>> Handle(GetAvailableOrdersQuery request, CancellationToken cancellationToken)
        {
            // Get orders with status "Confirmed" (waiting for driver assignment)
            var orders = await _unitOfWork.Orders.GetAllAsync();
            
            var availableOrders = orders
                .Where(o => o.Status == OrderStatus.Confirmed)
                .OrderBy(o => o.CreatedAt)
                .ToList();

            var totalCount = availableOrders.Count;
            var pagedOrders = availableOrders
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => _orderMapper.MapToDto(o))
                .ToList();

            return new PagedResponse<OrderDto>(
                data: pagedOrders,
                totalRecords: totalCount,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize
            );
        }
    }
}

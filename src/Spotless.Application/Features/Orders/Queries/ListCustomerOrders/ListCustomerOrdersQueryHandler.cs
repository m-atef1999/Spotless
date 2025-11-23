using MediatR;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace Spotless.Application.Features.Orders.Queries.ListCustomerOrders
{
    public class ListCustomerOrdersQueryHandler(IUnitOfWork unitOfWork, IOrderMapper orderMapper, Microsoft.Extensions.Logging.ILogger<ListCustomerOrdersQueryHandler> logger) : IRequestHandler<ListCustomerOrdersQuery, PagedResponse<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOrderMapper _orderMapper = orderMapper;
        private readonly Microsoft.Extensions.Logging.ILogger<ListCustomerOrdersQueryHandler> _logger = logger;

        public async Task<PagedResponse<OrderDto>> Handle(ListCustomerOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Build filter expression for customer orders
                Expression<Func<Order, bool>> filterExpression = order => order.CustomerId == request.CustomerId;

                var totalRecords = await _unitOfWork.Orders.CountAsync(filterExpression);

                var orders = await _unitOfWork.Orders.GetPagedAsync(
                    filterExpression,
                    (request.PageNumber - 1) * request.PageSize,  // Calculate skip from page number
                    request.PageSize,
                    include: query => query
                        .Include(o => o.Customer)
                        .Include(o => o.TimeSlot)
                        .Include(o => o.Items).ThenInclude(i => i.Service),
                    orderBy: q => q.OrderByDescending(o => o.OrderDate)
                );

                var orderDtos = _orderMapper.MapToDto(orders).ToList();

                return new PagedResponse<OrderDto>(
                    orderDtos,
                    totalRecords,
                    request.PageNumber,
                    request.PageSize
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing orders for customer {CustomerId}", request.CustomerId);
                throw;
            }
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Entities;
using System.Linq.Expressions;

namespace Spotless.Application.Features.Orders.Queries.GetOrderQuery
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PagedResponse<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderMapper _orderMapper;

        public GetOrdersQueryHandler(IUnitOfWork unitOfWork, IOrderMapper orderMapper)
        {
            _unitOfWork = unitOfWork;
            _orderMapper = orderMapper;
        }

        public async Task<PagedResponse<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var filterExpression = BuildFilterExpression(request);

            var totalRecords = await _unitOfWork.Orders.CountAsync(filterExpression);

            var orders = await _unitOfWork.Orders.GetPagedAsync(
                filterExpression,
                request.Skip,
                request.PageSize,
                include: query => query
                    .Include(o => o.Customer)
                    .Include(o => o.Items),
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

        private Expression<Func<Order, bool>> BuildFilterExpression(GetOrdersQuery request)
        {
            return order =>
                (string.IsNullOrEmpty(request.CustomerEmail) || order.Customer.Email.Contains(request.CustomerEmail!)) &&
                (!request.StatusFilter.HasValue || order.Status == request.StatusFilter.Value);
        }
    }
}
using MediatR;
using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Orders.Queries.GetOrdersAdmin
{
    public class GetOrdersAdminQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrdersAdminQuery, PagedResponse<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<PagedResponse<OrderDto>> Handle(GetOrdersAdminQuery request, CancellationToken cancellationToken)
        {
            var (orders, totalCount) = await _unitOfWork.Orders.GetOrdersForAdminAsync(
                request.PageNumber,
                request.PageSize,
                request.Status,
                request.SearchTerm
            );

            var orderDtos = orders.Select(o => new OrderDto(
                o.Id,
                o.CustomerId,
                o.Customer?.Name ?? "Unknown Customer",
                o.DriverId,
                o.TimeSlotId,
                null, // StartTime
                null, // EndTime
                o.ScheduledDate,
                o.PickupLocation.Latitude.GetValueOrDefault(),
                o.PickupLocation.Longitude.GetValueOrDefault(),
                o.PickupAddress,
                o.DeliveryLocation.Latitude.GetValueOrDefault(),
                o.DeliveryLocation.Longitude.GetValueOrDefault(),
                o.DeliveryAddress,
                o.TotalPrice.Amount,
                o.TotalPrice.Currency,
                o.Status,
                o.PaymentMethod,
                o.Items.FirstOrDefault()?.Service?.Name ?? "Unknown Service",
                o.OrderDate, // CreatedAt
                o.OrderDate,
                0, // EstimatedDurationHours
                new List<OrderItemDto>() // Items (not needed for list view)
            )).ToList();

            return new PagedResponse<OrderDto>(
                orderDtos,
                totalCount,
                request.PageNumber,
                request.PageSize
            );
        }
    }
}

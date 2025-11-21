using MediatR;
using Spotless.Application.Dtos.Analytics;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Analytics.Queries.GetRevenueReport
{
    public class GetRevenueReportQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRevenueReportQuery, RevenueReportDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<RevenueReportDto> Handle(GetRevenueReportQuery request, CancellationToken cancellationToken)
        {
            // Get completed orders within date range
            var orders = await _unitOfWork.Orders.GetAsync(o =>
                o.Status == OrderStatus.Delivered &&
                o.OrderDate >= request.StartDate &&
                o.OrderDate <= request.EndDate);

            var totalRevenue = orders.Sum(o => o.TotalPrice.Amount);
            var totalOrders = orders.Count;
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Group by date for daily breakdown
            var dailyBreakdown = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailyRevenueDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalPrice.Amount),
                    OrderCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            return new RevenueReportDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                AverageOrderValue = averageOrderValue,
                DailyBreakdown = dailyBreakdown
            };
        }
    }
}

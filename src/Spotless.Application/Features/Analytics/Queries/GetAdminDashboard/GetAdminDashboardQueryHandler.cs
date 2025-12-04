using MediatR;
using Spotless.Application.Dtos.Analytics;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Analytics.Queries.GetAdminDashboard
{
    public class GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Get all orders
            var allOrders = await _unitOfWork.Orders.GetAllAsync();
            
            // Get all drivers
            var allDrivers = await _unitOfWork.Drivers.GetAllAsync();
            
            // Get all customers
            var allCustomers = await _unitOfWork.Customers.GetAllAsync();
            
            // Get all services
            var allServices = await _unitOfWork.Services.GetAllAsync();
            
            // Get all categories
            var allCategories = await _unitOfWork.Categories.GetAllAsync();

            // Calculate metrics
            var totalOrders = allOrders.Count;
            var pendingOrders = allOrders.Count(o => o.Status == OrderStatus.Requested);
            var completedOrders = allOrders.Count(o => o.Status == OrderStatus.Delivered);
            var cancelledOrders = allOrders.Count(o => o.Status == OrderStatus.Cancelled);
            
            var activeDrivers = allDrivers.Count(d => d.Status == DriverStatus.Available);
            var totalDrivers = allDrivers.Count;
            
            var totalCustomers = allCustomers.Count;
            
            // Calculate revenue from completed orders
            var completedOrdersList = allOrders.Where(o => o.Status == OrderStatus.Delivered).ToList();
            var totalRevenue = completedOrdersList.Sum(o => o.TotalPrice.Amount);
            var monthlyRevenue = completedOrdersList
                .Where(o => o.OrderDate >= startOfMonth)
                .Sum(o => o.TotalPrice.Amount);

            // Get top 5 most used services from completed orders
            var topServices = await _unitOfWork.Orders.GetMostUsedServicesAsync(1, 5);

            return new AdminDashboardDto
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders,
                ActiveDrivers = activeDrivers,
                TotalDrivers = totalDrivers,
                TotalCustomers = totalCustomers,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                TotalServices = allServices.Count,
                TotalCategories = allCategories.Count,
                TopServices = topServices
            };
        }
    }
}

using MediatR;
using Spotless.Application.Dtos.Admin;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Admins.Queries.GetAdminDashboard
{
    public class GetAdminDashboardQueryHandler(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IDriverRepository driverRepository,
        ICustomerRepository customerRepository) 
        : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IPaymentRepository _paymentRepository = paymentRepository;
        private readonly IDriverRepository _driverRepository = driverRepository;
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var todayEnd = today.AddDays(1);

            // Run queries sequentially - DbContext is NOT thread-safe
            var totalOrdersToday = await _orderRepository.CountAsync(o =>
                o.OrderDate >= today && o.OrderDate < todayEnd);

            var (revenueToday, revenueCurrency) = await _paymentRepository
                .GetRevenueSummaryAsync(today, todayEnd);

            var mostUsedServices = await _orderRepository
                .GetMostUsedServicesAsync(request.PageNumber, request.PageSize);

            var activeCleaners = await _driverRepository.CountAsync(d =>
                d.Status == DriverStatus.Available
                || d.Status == DriverStatus.DriverAssigned
                || d.Status == DriverStatus.OnRoute
                || d.Status == DriverStatus.Busy);

            var newRegistrationsToday = await _customerRepository.CountAsync(c =>
                c.CreatedAt >= today && c.CreatedAt < todayEnd);

            var pendingBookings = await _orderRepository.CountAsync(o =>
                o.Status == OrderStatus.Requested
                || o.Status == OrderStatus.Confirmed
                || o.Status == OrderStatus.DriverAssigned
                || o.Status == OrderStatus.PickedUp
                || o.Status == OrderStatus.InCleaning
                || o.Status == OrderStatus.OutForDelivery);

            var totalRevenue = await _paymentRepository.GetTotalRevenueAsync();

            return new AdminDashboardDto(
                TotalOrdersToday: totalOrdersToday,
                RevenueToday: revenueToday,
                RevenueCurrency: revenueCurrency,
                MostUsedServices: mostUsedServices,
                NumberOfActiveCleaners: activeCleaners,
                NewRegistrationsToday: newRegistrationsToday,
                PendingBookings: pendingBookings,
                TotalRevenue: totalRevenue
            );
        }
    }
}

using MediatR;
using Spotless.Application.Dtos.Admin;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;


namespace Spotless.Application.Features.Admins.Queries.GetAdminDashboard
{

    public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;


        public GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;
            var todayEnd = today.AddDays(1);


            var totalOrdersToday = await _unitOfWork.Orders.CountAsync(o =>
                o.OrderDate >= today && o.OrderDate < todayEnd);


            var completedPaymentsToday = await _unitOfWork.Payments
                .GetCompletedPaymentsInDateRangeAsync(today, todayEnd);

            var revenueByCurrency = completedPaymentsToday
                .GroupBy(p => p.Amount.Currency)
                .Select(g => new { Currency = g.Key, Total = g.Sum(p => p.Amount.Amount) })
                .OrderByDescending(x => x.Total)
                .FirstOrDefault();

            var revenueToday = revenueByCurrency?.Total ?? 0m;
            var revenueCurrency = revenueByCurrency?.Currency ?? "EGP";



            var mostUsedServices = await _unitOfWork.Orders
                .GetMostUsedServicesAsync(10);



            var activeCleaners = await _unitOfWork.Drivers.CountAsync(d =>
                d.Status == DriverStatus.Available
                || d.Status == DriverStatus.DriverAssigned
                || d.Status == DriverStatus.OnRoute
                || d.Status == DriverStatus.Busy);


            var newRegistrationsToday = await _unitOfWork.Customers.CountAsync(c =>
                c.CreatedAt >= today && c.CreatedAt < todayEnd);


            var pendingBookings = await _unitOfWork.Orders.CountAsync(o =>
                o.Status == OrderStatus.Requested
                || o.Status == OrderStatus.Confirmed
                || o.Status == OrderStatus.DriverAssigned
                || o.Status == OrderStatus.PickedUp
                || o.Status == OrderStatus.InCleaning
                || o.Status == OrderStatus.OutForDelivery);

            return new AdminDashboardDto(
                TotalOrdersToday: totalOrdersToday,
                RevenueToday: revenueToday,
                RevenueCurrency: revenueCurrency,
                MostUsedServices: mostUsedServices,
                NumberOfActiveCleaners: activeCleaners,
                NewRegistrationsToday: newRegistrationsToday,
                PendingBookings: pendingBookings
            );
        }
    }
}
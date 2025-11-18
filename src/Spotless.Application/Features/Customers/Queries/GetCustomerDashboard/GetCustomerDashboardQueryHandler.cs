using MediatR;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Customer;
using Spotless.Application.Dtos.Payment;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Customers.Queries.GetCustomerDashboard
{
    public class GetCustomerDashboardQueryHandler : IRequestHandler<GetCustomerDashboardQuery, CustomerDashboardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderMapper _orderMapper;

        public GetCustomerDashboardQueryHandler(
            IUnitOfWork unitOfWork,
            IOrderMapper orderMapper)
        {
            _unitOfWork = unitOfWork;
            _orderMapper = orderMapper;
        }

        public async Task<CustomerDashboardDto> Handle(GetCustomerDashboardQuery request, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");
            }

            var today = DateTime.UtcNow.Date;


            var totalOrders = await _unitOfWork.Orders.CountAsync(o => o.CustomerId == request.CustomerId);


            var upcomingOrders = await _unitOfWork.Orders.GetPagedAsync(
                filter: o => o.CustomerId == request.CustomerId
                    && o.ScheduledDate.Date >= today
                    && o.Status != OrderStatus.Cancelled
                    && o.Status != OrderStatus.Delivered,
                skip: 0,
                take: 50,
                include: query => query.Include(o => o.Items),
                orderBy: q => q.OrderBy(o => o.ScheduledDate)
            );

            var upcomingBookedServicesCount = upcomingOrders.Count;
            var upcomingOrderDtos = _orderMapper.MapToDto(upcomingOrders).ToList();


            var walletBalance = customer.WalletBalance.Amount;
            var walletCurrency = customer.WalletBalance.Currency;


            var allPayments = await _unitOfWork.Payments.GetAsync(p => p.CustomerId == request.CustomerId);
            var recentPayments = allPayments
                .OrderByDescending(p => p.PaymentDate)
                .Take(10)
                .ToList();

            var recentTransactionDtos = recentPayments.Select(p => new PaymentDto(
                Id: p.Id,
                OrderId: p.OrderId ?? Guid.Empty,
                Amount: p.Amount.Amount,
                Currency: p.Amount.Currency,
                PaymentDate: p.PaymentDate,
                Method: p.PaymentMethod,
                Status: p.Status
            )).ToList();


            var totalReviewsSent = await _unitOfWork.Reviews.CountAsync(r => r.CustomerId == request.CustomerId);

            return new CustomerDashboardDto(
                TotalOrders: totalOrders,
                UpcomingBookedServices: upcomingBookedServicesCount,
                WalletBalance: walletBalance,
                WalletCurrency: walletCurrency,
                UpcomingOrders: upcomingOrderDtos,
                RecentTransactions: recentTransactionDtos,
                TotalReviewsSent: totalReviewsSent
            );
        }
    }
}


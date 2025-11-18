using Spotless.Application.Dtos.Order;
using Spotless.Application.Dtos.Payment;

namespace Spotless.Application.Dtos.Customer
{
    public record CustomerDashboardDto(
        int TotalOrders,
        int UpcomingBookedServices,
        decimal WalletBalance,
        string WalletCurrency,
        IReadOnlyList<OrderDto> UpcomingOrders,
        IReadOnlyList<PaymentDto> RecentTransactions,
        int TotalReviewsSent
    );
}


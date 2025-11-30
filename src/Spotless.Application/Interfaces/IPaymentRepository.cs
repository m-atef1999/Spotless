using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(Guid orderId);
        Task<List<Payment>> GetCompletedPaymentsInDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync();
        Task<(decimal Total, string Currency)> GetRevenueSummaryAsync(DateTime startDate, DateTime endDate);
    }
}

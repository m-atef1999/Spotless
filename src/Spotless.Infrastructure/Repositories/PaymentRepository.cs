using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class PaymentRepository(ApplicationDbContext dbContext) : BaseRepository<Payment>(dbContext), IPaymentRepository
    {
        public async Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(Guid orderId)
        {
            return await _dbContext.Payments
                                   .Where(p => p.OrderId == orderId)
                                   .ToListAsync();
        }
        public async Task<List<Payment>> GetCompletedPaymentsInDateRangeAsync(DateTime startDate, DateTime endDate)
        {

            return await _dbContext.Payments
                .Where(p => p.Status == PaymentStatus.Completed &&
                            p.PaymentDate >= startDate &&
                            p.PaymentDate < endDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbContext.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .SumAsync(p => p.Amount.Amount);
        }

        public async Task<(decimal Total, string Currency)> GetRevenueSummaryAsync(DateTime startDate, DateTime endDate)
        {
            var result = await _dbContext.Payments
                .Where(p => p.Status == PaymentStatus.Completed &&
                            p.PaymentDate >= startDate &&
                            p.PaymentDate < endDate)
                .GroupBy(p => p.Amount.Currency)
                .Select(g => new { Currency = g.Key, Total = g.Sum(p => p.Amount.Amount) })
                .OrderByDescending(x => x.Total)
                .FirstOrDefaultAsync();

            return (result?.Total ?? 0m, result?.Currency ?? "EGP");
        }
    }
}

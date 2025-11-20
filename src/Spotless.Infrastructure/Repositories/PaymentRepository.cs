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
    }
}

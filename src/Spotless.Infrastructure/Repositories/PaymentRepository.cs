using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<Payment>> GetPaymentsByOrderIdAsync(Guid orderId)
        {
            return await _dbContext.Payments
                                   .Where(p => p.OrderId == orderId)
                                   .ToListAsync();
        }
    }
}

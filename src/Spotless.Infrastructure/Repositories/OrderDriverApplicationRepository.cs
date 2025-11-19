using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class OrderDriverApplicationRepository : BaseRepository<OrderDriverApplication>, IOrderDriverApplicationRepository
    {
        public OrderDriverApplicationRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<OrderDriverApplication>> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbContext.Set<OrderDriverApplication>()
                .Where(a => a.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<OrderDriverApplication>> GetByDriverIdAsync(Guid driverId)
        {
            return await _dbContext.Set<OrderDriverApplication>()
                .Where(a => a.DriverId == driverId)
                .ToListAsync();
        }
    }
}
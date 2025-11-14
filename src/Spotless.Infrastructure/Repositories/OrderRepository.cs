using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByCustomerIdAsync(Guid customerId, OrderStatus? statusFilter = null)
        {
            var query = _dbContext.Orders.Where(o => o.CustomerId == customerId);

            if (statusFilter.HasValue)
            {
                query = query.Where(o => o.Status == statusFilter.Value);
            }

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<Order?> GetOrderWithTrackingInfoAsync(Guid orderId)
        {

            return await _dbContext.Orders
                                   .Include(o => o.Customer)
                                   .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IReadOnlyList<Order>> GetAvailableOrdersForDriverAsync(Guid driverId)
        {

            return await _dbContext.Orders
                                   .Where(o => o.DriverId == driverId && o.Status == OrderStatus.PickedUp)
                                   .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Admin;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class OrderRepository(ApplicationDbContext dbContext, IDistributedLockService? lockService = null) : BaseRepository<Order>(dbContext), IOrderRepository
    {
        private readonly IDistributedLockService? _lockService = lockService;

        public async Task<IReadOnlyList<MostUsedServiceDto>> GetMostUsedServicesAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.OrderItems
                .Include(oi => oi.Service)
                .GroupBy(oi => new { oi.ServiceId, oi.Service.Name })
                .Select(g => new MostUsedServiceDto(
                    g.Key.ServiceId,
                    g.Key.Name,
                    g.Select(oi => oi.OrderId).Distinct().Count()
                ))
                .OrderByDescending(s => s.OrderCount)
                .Skip((pageNumber - 1) * pageSize) // pagination
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByCustomerIdAsync(Guid customerId, OrderStatus? statusFilter = null)
        {
            var query = _dbContext.Orders.Where(o => o.CustomerId == customerId);

            if (statusFilter.HasValue)
                query = query.Where(o => o.Status == statusFilter.Value);

            return await query
                .Include(o => o.TimeSlot)
                .Include(o => o.Items).ThenInclude(i => i.Service)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
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

        public async Task<int> CountOrdersBySlotAsync(Guid timeSlotId, DateTime scheduledDate)
        {
            var start = scheduledDate.Date;
            var end = start.AddDays(1);

            return await _dbContext.Orders
                                   .Where(o => o.TimeSlotId == timeSlotId && o.ScheduledDate >= start && o.ScheduledDate < end)
                                   .CountAsync();
        }

        public async Task<TimeSlot?> GetTimeSlotByIdAsync(Guid timeSlotId)
        {
            return await _dbContext.TimeSlots.FirstOrDefaultAsync(t => t.Id == timeSlotId);
        }

        public async Task AddOrderWithSlotLockAsync(Order order, Guid timeSlotId, DateTime scheduledDate, int maxCapacity)
        {
            // Use a serializable transaction to avoid race conditions / overbooking under concurrency.
            // This is a simple prototype approach; for scale consider Redis-based distributed locks.
            await using var tx = await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            var start = scheduledDate.Date;
            var end = start.AddDays(1);

            var existingCount = await _dbContext.Orders
                                   .Where(o => o.TimeSlotId == timeSlotId && o.ScheduledDate >= start && o.ScheduledDate < end)
                                   .CountAsync();

            if (existingCount >= maxCapacity)
            {
                throw new InvalidOperationException("Selected time slot is full. Please choose another time.");
            }

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            await tx.CommitAsync();
        }

        public async Task AddOrderWithRedisLockAsync(Order order, Guid timeSlotId, DateTime scheduledDate, int maxCapacity)
        {
            if (_lockService == null)
            {
                throw new InvalidOperationException("Distributed lock service is not configured.");
            }

            // Create a unique lock key based on time slot and date
            var lockKey = $"timeslot:{timeSlotId}:{scheduledDate:yyyy-MM-dd}";

            // Execute the order creation within the distributed lock
            await _lockService.ExecuteWithLockAsync(lockKey, async () =>
            {
                var start = scheduledDate.Date;
                var end = start.AddDays(1);

                var existingCount = await _dbContext.Orders
                                       .Where(o => o.TimeSlotId == timeSlotId && o.ScheduledDate >= start && o.ScheduledDate < end)
                                       .CountAsync();

                if (existingCount >= maxCapacity)
                {
                    throw new InvalidOperationException("Selected time slot is full. Please choose another time.");
                }

                _dbContext.Orders.Add(order);
                await _dbContext.SaveChangesAsync();
                return true;
            });
        }
    }
}

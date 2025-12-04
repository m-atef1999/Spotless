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
            // Fetch the raw data from database first (only what we need)
            var orderItems = await _dbContext.OrderItems
                .Include(oi => oi.Service)
                .Where(oi => oi.Service != null) // Ensure service exists
                .Select(oi => new { oi.ServiceId, ServiceName = oi.Service.Name, oi.OrderId })
                .ToListAsync();

            // Perform the complex GroupBy with Distinct on client side
            var result = orderItems
                .GroupBy(oi => new { oi.ServiceId, oi.ServiceName })
                .Select(g => new MostUsedServiceDto(
                    g.Key.ServiceId,
                    g.Key.ServiceName ?? "Unknown",
                    g.Select(oi => oi.OrderId).Distinct().Count()
                ))
                .OrderByDescending(s => s.OrderCount)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return result;
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
            // Return ALL orders assigned to this driver (for order history)
            // Including completed and cancelled orders
            return await _dbContext.Orders
                                   .Where(o => o.DriverId == driverId)
                                   .Include(o => o.Items).ThenInclude(i => i.Service)
                                   .Include(o => o.TimeSlot)
                                   .OrderByDescending(o => o.OrderDate)
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


        public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetOrdersForAdminAsync(int pageNumber, int pageSize, OrderStatus? status, string? searchTerm)
        {
            var query = _dbContext.Orders.AsQueryable();

            // Apply Status Filter
            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            // Apply Search Filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                
                if (Guid.TryParse(term, out var searchId))
                {
                    // Exact match for ID
                    query = query.Where(o => o.Id == searchId);
                }
                else
                {
                    // Search by Service Name (via Items) or Customer Name

                    query = query.Where(o => 
                        o.Items.Any(i => i.Service.Name.ToLower().Contains(term)) ||
                        o.Customer.Name.ToLower().Contains(term)
                    );
                }
            }

            // Get Total Count before pagination
            var totalCount = await query.CountAsync();

            // Apply Sorting and Pagination
            var items = await query
                .Include(o => o.Customer)
                .Include(o => o.Driver)
                .Include(o => o.Items).ThenInclude(i => i.Service)
                .OrderByDescending(o => o.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}

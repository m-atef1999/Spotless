using Spotless.Application.Dtos.Admin;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetOrdersByCustomerIdAsync(Guid customerId, OrderStatus? statusFilter = null);
        Task<Order?> GetOrderWithTrackingInfoAsync(Guid orderId);
        Task<IReadOnlyList<Order>> GetAvailableOrdersForDriverAsync(Guid driverId);

        Task<IReadOnlyList<MostUsedServiceDto>> GetMostUsedServicesAsync(int pageNumber, int pageSize);
        Task<int> CountOrdersBySlotAsync(Guid timeSlotId, DateTime scheduledDate);
        Task<TimeSlot?> GetTimeSlotByIdAsync(Guid timeSlotId);
        Task AddOrderWithSlotLockAsync(Spotless.Domain.Entities.Order order, Guid timeSlotId, DateTime scheduledDate, int maxCapacity);
        
        /// <summary>
        /// Creates an order using Redis-based distributed locking for safe concurrent time-slot booking.
        /// </summary>
        Task AddOrderWithRedisLockAsync(Spotless.Domain.Entities.Order order, Guid timeSlotId, DateTime scheduledDate, int maxCapacity);
    }
}

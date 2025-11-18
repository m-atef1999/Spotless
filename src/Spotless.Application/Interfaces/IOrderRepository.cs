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

        Task<IReadOnlyList<MostUsedServiceDto>> GetMostUsedServicesAsync(int count);
    }
}
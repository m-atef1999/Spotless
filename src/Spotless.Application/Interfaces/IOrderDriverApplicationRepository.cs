using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface IOrderDriverApplicationRepository : IRepository<OrderDriverApplication>
    {
        Task<IReadOnlyList<OrderDriverApplication>> GetByOrderIdAsync(Guid orderId);
        Task<IReadOnlyList<OrderDriverApplication>> GetByDriverIdAsync(Guid driverId);
    }
}
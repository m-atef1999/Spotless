using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface IDriverApplicationRepository : IRepository<DriverApplication>
    {
        Task<DriverApplication?> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<DriverApplication>> GetAllPendingAsync();
    }
}

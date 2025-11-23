using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Interfaces
{
    public interface IDriverRepository : IRepository<Driver>
    {
        Task<IReadOnlyList<Driver>> GetDriversByStatusAsync(DriverStatus status);
        Task<Driver?> GetByEmailAsync(string email);
    }
}

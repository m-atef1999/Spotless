using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetByEmailAsync(string email);
    }
}

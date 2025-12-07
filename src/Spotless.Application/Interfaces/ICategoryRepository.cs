using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category> 
    {
        Task<IReadOnlyList<Category>> GetAllWithServicesAsync();
    }
}

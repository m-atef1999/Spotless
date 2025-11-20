using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface IServiceRepository : IRepository<Service>
    {
        Task<IReadOnlyList<Service>> GetServicesByCategoryId(Guid categoryId);
        Task<IReadOnlyList<Service>> GetFeaturedServicesAsync();
    }
}

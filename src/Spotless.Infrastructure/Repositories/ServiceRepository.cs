using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class ServiceRepository(ApplicationDbContext dbContext) : BaseRepository<Service>(dbContext), IServiceRepository
    {
        public async Task<IReadOnlyList<Service>> GetServicesByCategoryId(Guid categoryId)
        {
            return await _dbContext.Services
                                   .Where(s => s.CategoryId == categoryId)
                                   .ToListAsync();
        }
        
        public async Task<IReadOnlyList<Service>> GetFeaturedServicesAsync()
        {
            return await _dbContext.Services
                                   .Where(s => s.IsFeatured && s.IsActive)
                                   .ToListAsync();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class CategoryRepository(ApplicationDbContext dbContext) : BaseRepository<Category>(dbContext), ICategoryRepository
    {
        public async Task<IReadOnlyList<Category>> GetAllWithServicesAsync()
        {
            return await _dbContext.Set<Category>()
                .Include(c => c.Services)
                .ToListAsync();
        }
    }
}

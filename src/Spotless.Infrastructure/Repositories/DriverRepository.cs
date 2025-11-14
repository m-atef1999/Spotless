using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Spotless.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Spotless.Infrastructure.Repositories
{
    public class DriverRepository : BaseRepository<Driver>, IDriverRepository
    {
        public DriverRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<IReadOnlyList<Driver>> GetDriversByStatusAsync(DriverStatus status)
        {
            return await _dbContext.Drivers.Where(d => d.Status == status).ToListAsync();
        }
    }
}

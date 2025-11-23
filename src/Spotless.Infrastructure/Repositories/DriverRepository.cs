using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Spotless.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Spotless.Infrastructure.Repositories
{
    public class DriverRepository(ApplicationDbContext dbContext) : BaseRepository<Driver>(dbContext), IDriverRepository
    {
        public async Task<IReadOnlyList<Driver>> GetDriversByStatusAsync(DriverStatus status)
        {
            return await _dbContext.Drivers.Where(d => d.Status == status).ToListAsync();
        }

        public async Task<Driver?> GetByEmailAsync(string email)
        {
            return await _dbContext.Drivers.FirstOrDefaultAsync(d => d.Email == email);
        }
    }
}

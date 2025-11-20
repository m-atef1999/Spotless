using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class AdminRepository(ApplicationDbContext dbContext) : BaseRepository<Admin>(dbContext), IAdminRepository
    {
        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }
    }
}

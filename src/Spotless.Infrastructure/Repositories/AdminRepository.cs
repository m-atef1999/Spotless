using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email);
        }
    }
}

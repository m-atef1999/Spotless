using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class DriverApplicationRepository : BaseRepository<DriverApplication>, IDriverApplicationRepository
    {
        public DriverApplicationRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<DriverApplication?> GetByCustomerIdAsync(Guid customerId)
        {
            return await _dbContext.DriverApplications
                .OrderByDescending(da => da.CreatedAt)
                .FirstOrDefaultAsync(da => da.CustomerId == customerId);
        }

        public async Task<IEnumerable<DriverApplication>> GetAllPendingAsync()
        {
            return await _dbContext.DriverApplications
                .Where(da => da.Status == DriverApplicationStatus.Submitted || da.Status == DriverApplicationStatus.UnderReview)
                .OrderByDescending(da => da.CreatedAt)
                .ToListAsync();
        }
    }
}

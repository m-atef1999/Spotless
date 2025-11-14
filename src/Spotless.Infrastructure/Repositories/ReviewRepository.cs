using Microsoft.EntityFrameworkCore;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Infrastructure.Context;

namespace Spotless.Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<double> GetAverageDriverRatingAsync(Guid driverId)
        {

            return await _dbContext.Reviews
                .Where(r => r.DriverId == driverId)
                .Select(r => (double)r.Rating)
                .DefaultIfEmpty(0)
                .AverageAsync();
        }
    }
}

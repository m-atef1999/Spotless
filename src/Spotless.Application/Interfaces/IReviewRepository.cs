using Spotless.Domain.Entities;

namespace Spotless.Application.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<double> GetAverageDriverRatingAsync(Guid driverId);
    }
}

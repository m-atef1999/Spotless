using Spotless.Application.Dtos.Review;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Reviews.Queries.GetReviewByDriver
{

    public record GetReviewsByDriverQuery(
        Guid DriverId
    ) : IQuery<IReadOnlyList<ReviewDto>>;
}

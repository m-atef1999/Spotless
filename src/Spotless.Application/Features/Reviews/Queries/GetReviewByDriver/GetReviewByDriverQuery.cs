using MediatR;
using Spotless.Application.Dtos.Review;

namespace Spotless.Application.Features.Reviews.Queries.GetReviewByDriver
{

    public record GetReviewsByDriverQuery(
        Guid DriverId
    ) : IRequest<IReadOnlyList<ReviewDto>>;
}
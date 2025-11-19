using Spotless.Application.Dtos.Review;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Reviews.Queries.GetReviewsByCustomer
{
    public record GetReviewsByCustomerQuery(Guid CustomerId) : IQuery<IReadOnlyList<ReviewDto>>;
}
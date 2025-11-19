using Spotless.Application.Dtos.Review;
using Spotless.Application.Dtos.Requests;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Reviews.Queries.ListAllReviews
{
    public record GetAllReviewsQuery(string? SearchByComment) : PaginationBaseRequest, IQuery<PagedResponse<ReviewDto>>;
}
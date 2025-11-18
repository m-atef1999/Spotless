using MediatR;
using Spotless.Application.Dtos.Review;
using Spotless.Domain.Entities;

namespace Spotless.Application.Features.Reviews.Commands.CreateReview
{

    public record CreateReviewCommand(Guid CustomerId, CreateReviewDto Dto) : IRequest<Review>;
}
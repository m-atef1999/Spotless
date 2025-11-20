using MediatR;
using Spotless.Application.Dtos.Review;
using Spotless.Application.Interfaces;
using System.Linq;

namespace Spotless.Application.Features.Reviews.Queries.GetReviewByDriver
{

    public class GetReviewsByDriverQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetReviewsByDriverQuery, IReadOnlyList<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsByDriverQuery request, CancellationToken cancellationToken)
        {

            var reviews = await _unitOfWork.Reviews.GetAsync(
                review => review.DriverId.HasValue && review.DriverId.Value == request.DriverId
            );


            var reviewDtos = reviews.Select(review => new ReviewDto
            {
                Id = review.Id,
                CustomerId = review.CustomerId,
                OrderId = review.OrderId,
                DriverId = review.DriverId,
                Rating = review.Rating,
                Comment = review.Comment
            }).ToList();

            return reviewDtos;
        }
    }
}
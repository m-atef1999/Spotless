using MediatR;
using Spotless.Application.Dtos.Review;
using Spotless.Application.Interfaces;
using System.Linq;

namespace Spotless.Application.Features.Reviews.Queries.GetReviewsByCustomer
{
    public class GetReviewsByCustomerQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetReviewsByCustomerQuery, IReadOnlyList<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsByCustomerQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _unitOfWork.Reviews.GetAsync(r => r.CustomerId == request.CustomerId);

            var dtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                OrderId = r.OrderId,
                DriverId = r.DriverId,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();

            return dtos;
        }
    }
}
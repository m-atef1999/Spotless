using MediatR;
using Microsoft.EntityFrameworkCore;
using Spotless.Application.Dtos.Review;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using System.Linq;

namespace Spotless.Application.Features.Reviews.Queries.ListAllReviews
{
    public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, PagedResponse<ReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllReviewsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResponse<ReviewDto>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            System.Linq.Expressions.Expression<Func<Domain.Entities.Review, bool>> filterExpression;

            if (string.IsNullOrEmpty(request.SearchByComment))
            {
                filterExpression = r => true;
            }
            else
            {
                var search = request.SearchByComment;
                filterExpression = r => r.Comment != null && r.Comment.Contains(search);
            }

            var total = await _unitOfWork.Reviews.CountAsync(filterExpression);

            var reviews = await _unitOfWork.Reviews.GetPagedAsync(
                filterExpression,
                request.Skip,
                request.PageSize
            );

            var dtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                OrderId = r.OrderId,
                DriverId = r.DriverId,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();

            return new PagedResponse<ReviewDto>(dtos, total, request.PageNumber, request.PageSize);
        }
    }
}
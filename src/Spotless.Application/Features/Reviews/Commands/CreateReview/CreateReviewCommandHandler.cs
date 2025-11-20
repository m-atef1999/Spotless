using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;

namespace Spotless.Application.Features.Reviews.Commands.CreateReview
{

    public class CreateReviewCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateReviewCommand, Review>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Review> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {

            var order = await _unitOfWork.Orders.GetByIdAsync(request.Dto.OrderId) ?? throw new ApplicationException($"Order with ID {request.Dto.OrderId} not found.");
            var existingReview = (await _unitOfWork.Reviews.GetAsync(
                r => r.OrderId == request.Dto.OrderId && r.CustomerId == request.CustomerId
            )).FirstOrDefault();

            if (existingReview != null)
            {
                throw new ApplicationException($"Order with ID {request.Dto.OrderId} has already been reviewed by this customer.");
            }


            Guid? driverId = null;

            var review = new Review(
                customerId: request.CustomerId,
                orderId: request.Dto.OrderId,
                rating: request.Dto.Rating,
                comment: request.Dto.Comment,
                driverId: driverId
            );


            await _unitOfWork.Reviews.AddAsync(review);


            await _unitOfWork.CommitAsync();

            return review;
        }
    }
}
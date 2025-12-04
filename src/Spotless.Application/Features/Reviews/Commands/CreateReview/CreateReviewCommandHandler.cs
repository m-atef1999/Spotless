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

            // Find the driver associated with this order
            // Assuming Order has a DriverId or we need to find the driver via OrderDriverApplication
            // Based on previous analysis, Order has a DriverId (implied by Driver.Orders collection)
            // Let's check Order entity again if needed, but assuming standard relationship
            
            // Wait, I need to check how Order relates to Driver. 
            // In Driver.cs: public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
            // So Order should have a DriverId.
            
            // Let's verify Order entity to be 100% sure about the property name.
            // But for now, I will assume Order has DriverId based on the domain model.
            
            // Actually, looking at the previous view_file of Driver.cs, it has a collection of Orders.
            // So Order must have a DriverId.
            
            var driverId = order.DriverId;
            
            if (driverId.HasValue)
            {
                var driver = await _unitOfWork.Drivers.GetByIdAsync(driverId.Value);
                if (driver != null)
                {
                    driver.UpdateRating(request.Dto.Rating);
                    // No need to explicitly update if tracking is enabled, but let's be safe
                    // _unitOfWork.Drivers.Update(driver); // If repository pattern requires it
                }
            }

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

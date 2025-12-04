using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Queries.GetDriverEarnings
{
    public class GetDriverEarningsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetDriverEarningsQuery, DriverEarningsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<DriverEarningsDto> Handle(GetDriverEarningsQuery request, CancellationToken cancellationToken)
        {
            // Get the driver to retrieve their actual rating
            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);
            
            // Get all orders completed by this driver
            var orders = await _unitOfWork.Orders.GetAsync(o => o.DriverId == request.DriverId && o.Status == OrderStatus.Delivered);

            var totalEarnings = orders.Sum(o => o.TotalPrice.Amount) * 0.2m; // 20% commission
            var completedOrders = orders.Count;

            // Placeholder for pending payments (could be calculated based on payout status if that existed)
            var pendingPayments = totalEarnings; 

            // Use actual rating from driver entity
            var averageRating = driver?.AverageRating ?? 0.0;

            return new DriverEarningsDto
            {
                TotalEarnings = (double)totalEarnings,
                PendingPayments = (double)pendingPayments,
                CompletedOrders = completedOrders,
                AverageRating = averageRating
            };
        }
    }
}

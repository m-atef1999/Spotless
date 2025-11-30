using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Orders.Queries.GetOrderApplications
{
    public class GetOrderApplicationsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrderApplicationsQuery, List<OrderApplicationDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<OrderApplicationDto>> Handle(GetOrderApplicationsQuery request, CancellationToken cancellationToken)
        {
            var applications = await _unitOfWork.OrderDriverApplications.GetByOrderIdAsync(request.OrderId);

            // Assuming GetByOrderIdAsync includes Driver navigation property, otherwise we need to fetch drivers.
            // Based on previous code, it seemed to fetch drivers manually. Let's stick to that if navigation is not guaranteed.
            // But wait, previous code fetched drivers manually.
            
            var driverIds = applications.Select(a => a.DriverId).Distinct().ToList();
            var drivers = (await _unitOfWork.Drivers.GetAsync(d => driverIds.Contains(d.Id))).ToList();

            return applications.Select(a => new OrderApplicationDto(
                a.Id,
                a.DriverId,
                drivers.FirstOrDefault(d => d.Id == a.DriverId)?.Name ?? string.Empty,
                a.Status,
                a.AppliedAt
            )).ToList();
        }
    }
}

using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;
using System.Linq;

namespace Spotless.Application.Features.Orders.Queries.GetOrderApplications
{
    public class GetOrderApplicationsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrderApplicationsQuery, IReadOnlyList<DriverApplicationDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IReadOnlyList<DriverApplicationDto>> Handle(GetOrderApplicationsQuery request, CancellationToken cancellationToken)
        {
            var applications = await _unitOfWork.OrderDriverApplications.GetByOrderIdAsync(request.OrderId);

            var driverIds = applications.Select(a => a.DriverId).Distinct().ToList();

            var drivers = (await _unitOfWork.Drivers.GetAsync(d => driverIds.Contains(d.Id))).ToList();

            var dtos = applications.Select(a => new DriverApplicationDto(
                a.Id,
                a.DriverId,
                drivers.FirstOrDefault(d => d.Id == a.DriverId)?.Name ?? string.Empty,
                a.Status,
                a.AppliedAt
            )).ToList();

            return dtos;
        }
    }
}
using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Dtos.Responses;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Queries.GetDriverApplications
{
    public class GetDriverApplicationsQueryHandler(
        IRepository<DriverApplication> driverApplicationRepository,
        ICustomerRepository customerRepository) 
        : IRequestHandler<GetDriverApplicationsQuery, PagedResponse<DriverApplicationDto>>
    {
        private readonly IRepository<DriverApplication> _driverApplicationRepository = driverApplicationRepository;
        private readonly ICustomerRepository _customerRepository = customerRepository;

        public async Task<PagedResponse<DriverApplicationDto>> Handle(GetDriverApplicationsQuery request, CancellationToken cancellationToken)
        {
            // Build filter expression
            System.Linq.Expressions.Expression<Func<DriverApplication, bool>> filter = da => true;
            
            if (request.Status.HasValue)
            {
                filter = da => da.Status == request.Status.Value;
            }

            var totalCount = await _driverApplicationRepository.CountAsync(filter);
            
            var applications = await _driverApplicationRepository.GetPagedAsync(
                filter,
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                null,
                query => query.OrderByDescending(da => da.CreatedAt)
            );

            // Fetch customer details
            var customerIds = applications.Select(da => da.CustomerId).Distinct().ToList();
            var customers = (await _customerRepository.GetAsync(c => customerIds.Contains(c.Id))).ToDictionary(c => c.Id);

            var dtos = applications.Select(da =>
            {
                var customer = customers.GetValueOrDefault(da.CustomerId);
                return new DriverApplicationDto
                {
                    Id = da.Id,
                    CustomerId = da.CustomerId,
                    CustomerName = customer?.Name ?? "Unknown",
                    CustomerEmail = customer?.Email ?? "Unknown",
                    CustomerPhone = customer?.Phone ?? "Unknown",
                    VehicleInfo = da.VehicleInfo,
                    Status = da.Status.ToString(),
                    CreatedAt = da.CreatedAt,
                    UpdatedAt = da.UpdatedAt,
                    RejectionReason = da.RejectionReason
                };
            }).ToList();

            return new PagedResponse<DriverApplicationDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        }
    }
}

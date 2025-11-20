using MediatR;
using Spotless.Application.Dtos;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Queries.GetAvailableDrivers
{
    public class GetAvailableDriversQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAvailableDriversQuery, List<DriverDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<List<DriverDto>> Handle(GetAvailableDriversQuery request, CancellationToken cancellationToken)
        {
            var availableDrivers = await _unitOfWork.Drivers.GetAsync(
                driver => driver.Status == DriverStatus.Available
            );


            var driverDtos = availableDrivers.Select(driver => new DriverDto
            {
                Id = driver.Id,
                Name = driver.Name,
                Email = driver.Email,
                Phone = driver.Phone,
                VehicleInfo = driver.VehicleInfo,
                Status = driver.Status.ToString(),
                CurrentLocation = driver.CurrentLocation != null

                                  && driver.CurrentLocation.Latitude.HasValue
                                  && driver.CurrentLocation.Longitude.HasValue
                    ? new LocationDto
                    {
                        Latitude = driver.CurrentLocation.Latitude.Value,
                        Longitude = driver.CurrentLocation.Longitude.Value
                    }
                    : null
            }).ToList();

            return driverDtos;
        }
    }
}
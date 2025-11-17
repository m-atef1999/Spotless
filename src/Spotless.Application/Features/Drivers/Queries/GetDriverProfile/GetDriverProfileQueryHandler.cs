using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Drivers
{
    public class GetDriverProfileQueryHandler : IRequestHandler<GetDriverProfileQuery, DriverProfileDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDriverProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DriverProfileDto> Handle(GetDriverProfileQuery request, CancellationToken cancellationToken)
        {

            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);

            if (driver == null)
            {

                throw new KeyNotFoundException($"Driver profile with ID {request.DriverId} not found.");
            }


            return driver.ToProfileDto();
        }
    }
}

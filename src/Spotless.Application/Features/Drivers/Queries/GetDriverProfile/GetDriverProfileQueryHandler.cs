using MediatR;
using Spotless.Application.Dtos.Driver;
using Spotless.Application.Interfaces;
using Spotless.Application.Mappers;

namespace Spotless.Application.Features.Drivers.Queries
{
    public class GetDriverProfileQueryHandler : IRequestHandler<GetDriverProfileQuery, DriverProfileDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDriverMapper _driverMapper;

        public GetDriverProfileQueryHandler(IUnitOfWork unitOfWork, IDriverMapper driverMapper)
        {
            _unitOfWork = unitOfWork;
            _driverMapper = driverMapper;
        }

        public async Task<DriverProfileDto> Handle(GetDriverProfileQuery request, CancellationToken cancellationToken)
        {

            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);

            if (driver == null)
            {
                throw new KeyNotFoundException($"Driver with ID {request.DriverId} not found.");
            }


            return _driverMapper.MapToProfileDto(driver);
        }
    }
}
using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Drivers
{
    public class UpdateDriverProfileCommandHandler : IRequestHandler<UpdateDriverProfileCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDriverProfileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateDriverProfileCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);

            if (driver == null)
            {
                throw new KeyNotFoundException($"Driver profile not found for ID: {request.DriverId}");
            }



            string newName = dto.Name ?? driver.Name;
            string? newPhone = dto.Phone ?? driver.Phone;
            string newVehicleInfo = dto.VehicleInfo ?? driver.VehicleInfo;



            driver.UpdateProfile(
                newName,
                newPhone,
                newVehicleInfo
            );

            await _unitOfWork.Drivers.UpdateAsync(driver);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
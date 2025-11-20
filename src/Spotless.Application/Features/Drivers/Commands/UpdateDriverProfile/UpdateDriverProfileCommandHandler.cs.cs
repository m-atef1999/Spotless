using MediatR;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverProfile
{
    public class UpdateDriverProfileCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateDriverProfileCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateDriverProfileCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId) ?? throw new KeyNotFoundException($"Driver profile not found for ID: {request.DriverId}");
            string newName = string.IsNullOrWhiteSpace(dto.Name) ? driver.Name : dto.Name;


            string? newPhone = string.IsNullOrWhiteSpace(dto.Phone) ? driver.Phone : dto.Phone;

            string newVehicleInfo = string.IsNullOrWhiteSpace(dto.VehicleInfo) ? driver.VehicleInfo : dto.VehicleInfo;


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
using MediatR;
using Spotless.Application.Dtos;
using Spotless.Application.Interfaces;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverLocation
{
    public class UpdateDriverLocationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateDriverLocationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateDriverLocationCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId) ?? throw new KeyNotFoundException($"Driver not found: {request.DriverId}");
            driver.UpdateLocation(request.Location.Latitude, request.Location.Longitude);
            await _unitOfWork.Drivers.UpdateAsync(driver);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

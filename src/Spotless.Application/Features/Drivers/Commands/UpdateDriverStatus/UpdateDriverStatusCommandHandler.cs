using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Commands.UpdateDriverStatus
{
    public class UpdateDriverStatusCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateDriverStatusCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(UpdateDriverStatusCommand request, CancellationToken cancellationToken)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId) ?? throw new KeyNotFoundException($"Driver not found: {request.DriverId}");
            if (!Enum.TryParse<DriverStatus>(request.Status, true, out var status))
                throw new ArgumentException($"Invalid status: {request.Status}");

            driver.UpdateStatus(status);
            await _unitOfWork.Drivers.UpdateAsync(driver);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}

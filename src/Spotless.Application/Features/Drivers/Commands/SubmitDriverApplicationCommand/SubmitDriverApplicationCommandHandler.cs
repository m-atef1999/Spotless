using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Entities;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Commands.SubmitDriverApplicationCommand
{
    public class SubmitDriverApplicationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<SubmitDriverApplicationCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(SubmitDriverApplicationCommand request, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId) ?? throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");
            var newDriverApplication = new Driver(
                adminId: null,
                name: request.Dto.Name,
                email: request.Dto.Email,
                phone: request.Dto.Phone,
                vehicleInfo: request.Dto.VehicleInfo
            );


            newDriverApplication.UpdateStatus(DriverStatus.PendingApproval);


            await _unitOfWork.Drivers.AddAsync(newDriverApplication);
            await _unitOfWork.CommitAsync();

            return newDriverApplication.Id;
        }
    }
}
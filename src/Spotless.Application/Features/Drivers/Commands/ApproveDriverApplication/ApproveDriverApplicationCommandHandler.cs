using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Commands.ApproveDriverApplication
{
    public class ApproveDriverApplicationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ApproveDriverApplicationCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Guid> Handle(ApproveDriverApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.DriverApplications.GetByIdAsync(request.ApplicationId) 
                ?? throw new KeyNotFoundException($"Driver application with ID {request.ApplicationId} not found.");

            if (application.Status != DriverApplicationStatus.Submitted)
            {
                throw new InvalidOperationException($"Application cannot be approved. Current status is {application.Status}.");
            }

            application.Approve(request.AdminId);
            
            await _unitOfWork.DriverApplications.UpdateAsync(application);
            await _unitOfWork.CommitAsync();

            return application.Id;
        }
    }
}

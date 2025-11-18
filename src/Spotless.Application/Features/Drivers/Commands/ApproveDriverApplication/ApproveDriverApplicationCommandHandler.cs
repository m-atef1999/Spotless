using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Commands.ApproveDriverApplication
{
    public class ApproveDriverApplicationCommandHandler : IRequestHandler<ApproveDriverApplicationCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _identityService;

        public ApproveDriverApplicationCommandHandler(IUnitOfWork unitOfWork, IAuthService identityService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
        }

        public async Task<Guid> Handle(ApproveDriverApplicationCommand request, CancellationToken cancellationToken)
        {

            var driver = await _unitOfWork.Drivers.GetByIdAsync(request.ApplicationId);

            if (driver == null)
            {
                throw new KeyNotFoundException($"Driver application with ID {request.ApplicationId} not found.");
            }


            if (driver.Status != DriverStatus.PendingApproval && driver.Status != DriverStatus.Offline)
            {
                throw new InvalidOperationException($"Driver with ID {request.ApplicationId} cannot be approved. Current status is {driver.Status}.");
            }


            Guid userId = await _identityService.CreateUserAsync(
                driver.Email,
                request.Password,
                UserRole.Driver.ToString());


            driver.SetIdentityId(userId);



            driver.SetAdminId(request.AdminId);


            driver.UpdateStatus(DriverStatus.Offline);


            await _unitOfWork.Drivers.UpdateAsync(driver);
            await _unitOfWork.CommitAsync();

            return driver.Id;
        }
    }
}
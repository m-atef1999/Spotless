using MediatR;
using Spotless.Application.Interfaces;
using Spotless.Domain.Enums;

namespace Spotless.Application.Features.Drivers.Commands.RejectDriverRegistration
{
    public class RejectDriverRegistrationCommandHandler(IUnitOfWork unitOfWork, INotificationService notificationService, IAuthService authService) : IRequestHandler<RejectDriverRegistrationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IAuthService _authService = authService;

        public async Task<Unit> Handle(RejectDriverRegistrationCommand request, CancellationToken cancellationToken)
        {
            var application = await _unitOfWork.DriverApplications.GetByIdAsync(request.ApplicationId)
                ?? throw new KeyNotFoundException($"Driver application with ID {request.ApplicationId} not found.");

            if (application.Status != DriverApplicationStatus.Submitted && application.Status != DriverApplicationStatus.UnderReview)
            {
                throw new InvalidOperationException($"Cannot reject application with status {application.Status}.");
            }

            application.Reject(request.AdminId, request.Reason);
            await _unitOfWork.DriverApplications.UpdateAsync(application);
            await _unitOfWork.CommitAsync();

            // Notify Customer
            var userIdString = await _authService.GetUserIdByCustomerIdAsync(application.CustomerId);
            if (userIdString != null)
            {
                await _notificationService.SendPushNotificationAsync(userIdString, "Application Rejected", $"Your driver application was rejected. Reason: {request.Reason}");
            }

            return Unit.Value;
        }
    }
}

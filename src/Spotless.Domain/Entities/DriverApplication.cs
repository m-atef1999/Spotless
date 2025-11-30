using Spotless.Domain.Enums;

namespace Spotless.Domain.Entities
{
    public class DriverApplication : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;
        public DriverApplicationStatus Status { get; private set; }
        public string VehicleInfo { get; private set; } = string.Empty;
        public Guid? ReviewedBy { get; private set; }
        public string? RejectionReason { get; private set; }

        protected DriverApplication() { }

        public DriverApplication(Guid customerId, string vehicleInfo)
        {
            CustomerId = customerId;
            VehicleInfo = vehicleInfo;
            Status = DriverApplicationStatus.Submitted;
        }

        public void Approve(Guid reviewerId)
        {
            Status = DriverApplicationStatus.Approved;
            ReviewedBy = reviewerId;
            MarkAsUpdated();
        }

        public void Reject(Guid reviewerId, string reason)
        {
            Status = DriverApplicationStatus.Rejected;
            ReviewedBy = reviewerId;
            RejectionReason = reason;
            MarkAsUpdated();
        }

        public void Resubmit(string vehicleInfo)
        {
            VehicleInfo = vehicleInfo;
            Status = DriverApplicationStatus.Submitted;
            RejectionReason = null;
            ReviewedBy = null;
            MarkAsUpdated();
        }
    }
}

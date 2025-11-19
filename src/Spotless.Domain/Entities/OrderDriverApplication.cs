using Spotless.Domain.Enums;

namespace Spotless.Domain.Entities
{
    public class OrderDriverApplication : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid DriverId { get; private set; }
        public OrderDriverApplicationStatus Status { get; private set; } = OrderDriverApplicationStatus.Applied;
        public DateTime AppliedAt { get; private set; } = DateTime.UtcNow;

        protected OrderDriverApplication() { }

        public OrderDriverApplication(Guid orderId, Guid driverId)
        {
            OrderId = orderId;
            DriverId = driverId;
            Status = OrderDriverApplicationStatus.Applied;
            AppliedAt = DateTime.UtcNow;
        }

        public void Accept()
        {
            Status = OrderDriverApplicationStatus.Accepted;
        }

        public void Reject()
        {
            Status = OrderDriverApplicationStatus.Rejected;
        }
    }
}
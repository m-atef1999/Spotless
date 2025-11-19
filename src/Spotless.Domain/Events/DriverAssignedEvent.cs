namespace Spotless.Domain.Events
{
    public class DriverAssignedEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public Guid OrderId { get; }
        public Guid DriverId { get; }
        public Guid CustomerId { get; }

        public DriverAssignedEvent(Guid orderId, Guid driverId, Guid customerId)
        {
            OrderId = orderId;
            DriverId = driverId;
            CustomerId = customerId;
        }
    }
}
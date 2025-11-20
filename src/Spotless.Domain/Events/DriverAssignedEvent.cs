namespace Spotless.Domain.Events
{
    public class DriverAssignedEvent(Guid orderId, Guid driverId, Guid customerId) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public Guid OrderId { get; } = orderId;
        public Guid DriverId { get; } = driverId;
        public Guid CustomerId { get; } = customerId;
    }
}

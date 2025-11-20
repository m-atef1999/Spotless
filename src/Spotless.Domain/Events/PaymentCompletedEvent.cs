namespace Spotless.Domain.Events
{
    public class PaymentCompletedEvent(Guid paymentId, Guid customerId, Guid? orderId, decimal amount) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public Guid PaymentId { get; } = paymentId;
        public Guid CustomerId { get; } = customerId;
        public Guid? OrderId { get; } = orderId;
        public decimal Amount { get; } = amount;
    }
}
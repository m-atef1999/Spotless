namespace Spotless.Domain.Events
{
    public class PaymentCompletedEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public Guid PaymentId { get; }
        public Guid CustomerId { get; }
        public Guid? OrderId { get; }
        public decimal Amount { get; }

        public PaymentCompletedEvent(Guid paymentId, Guid customerId, Guid? orderId, decimal amount)
        {
            PaymentId = paymentId;
            CustomerId = customerId;
            OrderId = orderId;
            Amount = amount;
        }
    }
}
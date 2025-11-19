namespace Spotless.Domain.Events
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
        public decimal TotalAmount { get; }

        public OrderCreatedEvent(Guid orderId, Guid customerId, decimal totalAmount)
        {
            OrderId = orderId;
            CustomerId = customerId;
            TotalAmount = totalAmount;
        }
    }
}
namespace Spotless.Domain.Events
{
    public class OrderCreatedEvent(Guid orderId, Guid customerId, decimal totalPrice) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public Guid OrderId { get; } = orderId;
        public Guid CustomerId { get; } = customerId;
        public decimal TotalPrice { get; } = totalPrice;
    }
}
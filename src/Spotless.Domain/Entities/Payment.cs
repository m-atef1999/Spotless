using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

namespace Spotless.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public Guid? AdminId { get; private set; }
        public Guid? OrderId { get; private set; }
        public Money Amount { get; private set; } = null!;
        public DateTime PaymentDate { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }

        public virtual Customer Customer { get; private set; } = null!;
        public virtual Admin? Admin { get; private set; }
        public virtual Order? Order { get; private set; }

        protected Payment() { }


        public Payment(Guid customerId, Money amount, PaymentMethod method, Guid? orderId = null, Guid? adminId = null) : base()
        {
            if (amount.Amount <= 0)
                throw new ArgumentException("Payment amount must be positive.", nameof(amount));

            CustomerId = customerId;
            OrderId = orderId;
            Amount = amount;
            PaymentMethod = method;
            AdminId = adminId;
            PaymentDate = DateTime.UtcNow;
            Status = PaymentStatus.Pending;
        }

        public void CompletePayment()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException($"Payment cannot be completed from status {Status}. Only Pending status is allowed.");

            Status = PaymentStatus.Completed;
        }


        public void FailPayment()
        {
            if (Status != PaymentStatus.Pending && Status != PaymentStatus.Failed)
                throw new InvalidOperationException($"Payment cannot be marked Failed from status {Status}. Only Pending is allowed.");

            Status = PaymentStatus.Failed;
        }

        public void Refund()
        {
            if (Status != PaymentStatus.Completed)
                throw new InvalidOperationException($"Payment can only be refunded if status is {PaymentStatus.Completed}.");

            Status = PaymentStatus.Refunded;
        }
    }
}
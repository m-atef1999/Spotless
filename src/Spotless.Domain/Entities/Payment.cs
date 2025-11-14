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

        public Payment(Guid customerId, Guid orderId, Money amount, PaymentMethod method, Guid? adminId = null) : base()
        {
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
                throw new InvalidOperationException("Payment cannot be completed unless it is Pending.");

            Status = PaymentStatus.Completed;
        }
    }
}

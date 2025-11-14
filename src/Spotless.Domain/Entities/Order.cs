using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;

namespace Spotless.Domain.Entities
{
    public class Order : BaseEntity
    {

        public Guid CustomerId { get; private set; }
        public Guid? DriverId { get; private set; }
        public Guid ServiceId { get; private set; }
        public Guid? AdminId { get; private set; }

        public Money TotalPrice { get; private set; } = null!;
        public DateTime PickupTime { get; private set; }
        public DateTime DeliveryTime { get; private set; }
        public OrderStatus Status { get; private set; } = OrderStatus.Requested;
        public PaymentMethod PaymentMethod { get; private set; }
        public DateTime OrderDate { get; private set; } = DateTime.UtcNow;

        public virtual Customer Customer { get; private set; } = null!;

        public virtual Service Service { get; private set; } = null!;

        protected Order() { }

        public Order(Guid customerId, Guid serviceId, Money totalPrice, DateTime pickupTime, DateTime deliveryTime, PaymentMethod paymentMethod) : base()
        {
            CustomerId = customerId;
            ServiceId = serviceId;
            TotalPrice = totalPrice;
            PickupTime = pickupTime;
            DeliveryTime = deliveryTime;
            PaymentMethod = paymentMethod;
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Requested;
        }

        public void AssignDriver(Guid driverId)
        {
            if (Status != OrderStatus.Requested)
                throw new InvalidOperationException("Cannot assign driver to an order that is not in a 'Requested' status.");

            DriverId = driverId;
        }

        public void SetStatus(OrderStatus newStatus)
        {

            if (this.Status == OrderStatus.Delivered && newStatus == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Completed orders cannot be cancelled.");
            }

            this.Status = newStatus;
        }
    }
}

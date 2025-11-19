using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;
using Spotless.Domain.Events;

namespace Spotless.Domain.Entities
{
    public class Order : BaseEntity
    {


        public Guid CustomerId { get; private set; }
        public Guid? DriverId { get; private set; }
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public Guid? AdminId { get; private set; }
        public Location PickupLocation { get; private set; } = null!;
        public Location DeliveryLocation { get; private set; } = null!;
        public Money TotalPrice { get; private set; } = null!;
        public Guid TimeSlotId { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public virtual TimeSlot TimeSlot { get; private set; } = null!;
        public OrderStatus Status { get; private set; } = OrderStatus.Requested;
        public PaymentMethod PaymentMethod { get; private set; }
        public DateTime OrderDate { get; private set; } = DateTime.UtcNow;

        public virtual Customer Customer { get; private set; } = null!;



        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        protected Order() { }


        public Order(Guid customerId, IEnumerable<OrderItem> items, Money totalPrice, Guid timeSlotId, DateTime scheduledDate, PaymentMethod paymentMethod, Location pickupLocation, Location deliveryLocation) : base()
        {
            if (!items.Any())
                throw new InvalidOperationException("An order must contain at least one service item.");

            CustomerId = customerId;

            _items.AddRange(items);

            TotalPrice = totalPrice;
            PaymentMethod = paymentMethod;
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Requested;
            TimeSlotId = timeSlotId;
            ScheduledDate = scheduledDate.Date;
            PickupLocation = pickupLocation;
            DeliveryLocation = deliveryLocation;
        }





        public void SetStatus(OrderStatus newStatus) { this.Status = newStatus; }


        public void UpdateDetails(Guid newTimeSlotId, DateTime newScheduledDate, Location newPickupLocation, Location newDeliveryLocation)
        {
            if (this.Status != OrderStatus.Requested || this.DriverId.HasValue)
            {
                throw new InvalidOperationException($"Cannot update order details. Status is {this.Status} or a driver is assigned.");
            }

            TimeSlotId = newTimeSlotId;
            ScheduledDate = newScheduledDate.Date;
            PickupLocation = newPickupLocation;
            DeliveryLocation = newDeliveryLocation;
        }


        public void AddItem(OrderItem item)
        {
            if (this.Status != OrderStatus.Requested)
                throw new InvalidOperationException("Cannot add items to an order that is not in the 'Requested' status.");

            _items.Add(item);

        }
        public void UpdatePaymentMethod(PaymentMethod newPaymentMethod)
        {

            PaymentMethod = newPaymentMethod;
        }
        public void UpdateService(Guid newServiceId)
        {
            if (this.Status != OrderStatus.Requested)
            {
                throw new InvalidOperationException($"Cannot change service. Status is {this.Status}.");
            }


            var firstItem = _items.FirstOrDefault();

            if (firstItem == null)
            {

                throw new InvalidOperationException("Cannot update service: Order contains no items.");
            }


            firstItem.UpdateServiceId(newServiceId);

        }
        public void AssignDriver(Guid driverId)
        {
            if (this.Status != OrderStatus.Confirmed && this.Status != OrderStatus.DriverAssigned)
            {
                throw new InvalidOperationException($"Cannot assign driver to order with status {this.Status}. Order must be Confirmed or DriverAssigned.");
            }
            if (driverId == Guid.Empty)
            {
                throw new ArgumentException("Driver ID cannot be empty.", nameof(driverId));
            }

            this.DriverId = driverId;
            this.Status = OrderStatus.DriverAssigned;
        }
        
        public DriverAssignedEvent CreateDriverAssignedEvent()
        {
            return new DriverAssignedEvent(this.Id, this.DriverId!.Value, this.CustomerId);
        }
        
        public OrderCreatedEvent CreateOrderCreatedEvent()
        {
            return new OrderCreatedEvent(this.Id, this.CustomerId, this.TotalPrice.Amount);
        }
    }
}

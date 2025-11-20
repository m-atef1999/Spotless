using Spotless.Domain.Enums;
using Spotless.Domain.ValueObjects;
using Spotless.Domain.Events;

namespace Spotless.Domain.Entities
{
    public class Order : BaseEntity
    {


        public Guid CustomerId { get; private set; }
        public Guid? DriverId { get; private set; }
        public virtual ICollection<Payment> Payments { get; set; } = [];

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



        private readonly List<OrderItem> _items = [];
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





        private static readonly Dictionary<OrderStatus, List<OrderStatus>> StateTransitions = new()
        {
            { OrderStatus.PaymentFailed, new List<OrderStatus> { OrderStatus.Requested, OrderStatus.Cancelled } },
            { OrderStatus.Requested, new List<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
            { OrderStatus.Confirmed, new List<OrderStatus> { OrderStatus.DriverAssigned, OrderStatus.Cancelled } },
            { OrderStatus.DriverAssigned, new List<OrderStatus> { OrderStatus.PickedUp, OrderStatus.Cancelled } },
            { OrderStatus.PickedUp, new List<OrderStatus> { OrderStatus.InCleaning, OrderStatus.Cancelled } },
            { OrderStatus.InCleaning, new List<OrderStatus> { OrderStatus.OutForDelivery, OrderStatus.Cancelled } },
            { OrderStatus.OutForDelivery, new List<OrderStatus> { OrderStatus.Delivered, OrderStatus.Cancelled } }
        };

        public void SetStatus(OrderStatus newStatus)
        {
            if (StateTransitions.TryGetValue(Status, out var validNextStatuses))
            {
                if (validNextStatuses.Contains(newStatus))
                {
                    Status = newStatus;
                }
                else
                {
                    throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}");
                }
            }
            else
            {
                // No transitions defined for the current status, meaning it's a final state (like Delivered or Cancelled)
                throw new InvalidOperationException($"Cannot transition from a final state {Status}");
            }
        }


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


            var firstItem = _items.FirstOrDefault() ?? throw new InvalidOperationException("Cannot update service: Order contains no items.");
            firstItem.UpdateServiceId(newServiceId);

        }

        public void AddPayment(Payment payment)
        {
            if (payment.Status != PaymentStatus.Completed)
            {
                throw new InvalidOperationException("Only completed payments can be added to an order.");
            }

            if (Payments.Any(p => p.Id == payment.Id))
            {
                throw new InvalidOperationException("Payment has already been added to this order.");
            }

            Payments.Add(payment);

            var totalPaid = Payments.Sum(p => p.Amount.Amount);

            if (totalPaid > TotalPrice.Amount)
            {
                throw new InvalidOperationException($"Overpayment detected. Total paid: {totalPaid}, Order total: {TotalPrice.Amount}");
            }

            if (totalPaid == TotalPrice.Amount)
            {
                SetStatus(OrderStatus.Confirmed);
            }
        }
        public void AssignDriver(Guid driverId)
        {
            if (driverId == Guid.Empty)
            {
                throw new ArgumentException("Driver ID cannot be empty.", nameof(driverId));
            }

            SetStatus(OrderStatus.DriverAssigned);
            this.DriverId = driverId;
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

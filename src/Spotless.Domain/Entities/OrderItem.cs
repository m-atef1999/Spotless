using Spotless.Domain.ValueObjects;

namespace Spotless.Domain.Entities
{
    public class OrderItem : BaseEntity
    {

        public Guid OrderId { get; private set; }
        public Guid ServiceId { get; private set; }


        public Money Price { get; private set; } = null!;
        public int Quantity { get; private set; }


        public virtual Order Order { get; private set; } = null!;
        public virtual Service Service { get; private set; } = null!;

        protected OrderItem() { }

        public OrderItem(Guid orderId, Guid serviceId, Money price, int quantity) : base()
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
            }

            OrderId = orderId;
            ServiceId = serviceId;
            Price = price;
            Quantity = quantity;
        }


        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be greater than zero.");
            }
            Quantity = newQuantity;
        }
    }
}

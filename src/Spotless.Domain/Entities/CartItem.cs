namespace Spotless.Domain.Entities
{
    /// <summary>
    /// Represents a single item in a shopping cart.
    /// </summary>
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; private set; }
        public Guid ServiceId { get; private set; }
        public int Quantity { get; private set; }
        public DateTime AddedDate { get; private set; } = DateTime.UtcNow;

        public virtual Cart Cart { get; private set; } = null!;
        public virtual Service Service { get; private set; } = null!;

        protected CartItem() { }

        public CartItem(Guid cartId, Guid serviceId, int quantity) : base()
        {
            if (cartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));

            if (serviceId == Guid.Empty)
                throw new ArgumentException("Service ID cannot be empty.", nameof(serviceId));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

            CartId = cartId;
            ServiceId = serviceId;
            Quantity = quantity;
        }

        /// <summary>
        /// Updates the quantity of this cart item.
        /// </summary>
        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be greater than zero.");

            Quantity = newQuantity;
        }
    }
}

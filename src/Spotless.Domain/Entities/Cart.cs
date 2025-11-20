namespace Spotless.Domain.Entities
{
    /// <summary>
    /// Represents a shopping cart for a customer.
    /// Supports adding multiple services before checkout.
    /// </summary>
    public class Cart : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; private set; } = DateTime.UtcNow;

        private readonly List<CartItem> _items = [];
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        protected Cart() { }

        public Cart(Guid customerId) : base()
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer ID cannot be empty.", nameof(customerId));

            CustomerId = customerId;
        }

        /// <summary>
        /// Adds or updates an item in the cart.
        /// If the service already exists, updates the quantity.
        /// </summary>
        public void AddOrUpdateItem(Guid serviceId, int quantity)
        {
            if (serviceId == Guid.Empty)
                throw new ArgumentException("Service ID cannot be empty.", nameof(serviceId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            var existingItem = _items.FirstOrDefault(item => item.ServiceId == serviceId);
            
            if (existingItem != null)
            {
                existingItem.UpdateQuantity(quantity);
            }
            else
            {
                _items.Add(new CartItem(this.Id, serviceId, quantity));
            }

            LastModifiedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Removes an item from the cart by service ID.
        /// </summary>
        public void RemoveItem(Guid serviceId)
        {
            if (serviceId == Guid.Empty)
                throw new ArgumentException("Service ID cannot be empty.", nameof(serviceId));

            var item = _items.FirstOrDefault(item => item.ServiceId == serviceId);
            
            if (item != null)
            {
                _items.Remove(item);
                LastModifiedDate = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Clears all items from the cart.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            LastModifiedDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the total weight across all cart items.
        /// </summary>
        public decimal CalculateTotalWeightKg(Dictionary<Guid, decimal> serviceMaxWeights)
        {
            decimal totalWeight = 0;

            foreach (var item in _items)
            {
                if (serviceMaxWeights.TryGetValue(item.ServiceId, out var maxWeight))
                {
                    totalWeight += maxWeight * item.Quantity;
                }
            }

            return totalWeight;
        }

        /// <summary>
        /// Validates if the cart is eligible for checkout.
        /// </summary>
        public bool IsValidForCheckout()
        {
            return _items.Count > 0;
        }
    }
}

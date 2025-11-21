using Spotless.Domain.Enums;

namespace Spotless.Domain.Entities
{
    public class PaymentMethodEntity : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public PaymentMethodType Type { get; private set; }
        public string Last4Digits { get; private set; } = string.Empty;
        public string CardholderName { get; private set; } = string.Empty;
        public DateTime ExpiryDate { get; private set; }
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        protected PaymentMethodEntity() { }

        public PaymentMethodEntity(
            Guid customerId,
            PaymentMethodType type,
            string last4Digits,
            string cardholderName,
            DateTime expiryDate,
            bool isDefault = false)
        {
            CustomerId = customerId;
            Type = type;
            Last4Digits = last4Digits;
            CardholderName = cardholderName;
            ExpiryDate = expiryDate;
            IsDefault = isDefault;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
        }

        public void UnsetAsDefault()
        {
            IsDefault = false;
        }
    }
}

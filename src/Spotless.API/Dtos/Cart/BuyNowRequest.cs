using Spotless.Domain.Enums;

namespace Spotless.API.Dtos.Cart
{
    public class BuyNowRequest
    {
        public Guid ServiceId { get; set; }
        public int Quantity { get; set; } = 1;

        public Guid TimeSlotId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public decimal PickupLatitude { get; set; }
        public decimal PickupLongitude { get; set; }
        public decimal DeliveryLatitude { get; set; }
        public decimal DeliveryLongitude { get; set; }
    }
}

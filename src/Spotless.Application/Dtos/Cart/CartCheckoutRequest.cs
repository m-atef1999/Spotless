using Spotless.Domain.Enums;

namespace Spotless.Application.Dtos.Cart
{
    public class CartCheckoutRequest
    {
        public Guid TimeSlotId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public decimal PickupLatitude { get; set; }
        public decimal PickupLongitude { get; set; }
        public string? PickupAddress { get; set; }
        public decimal DeliveryLatitude { get; set; }
        public decimal DeliveryLongitude { get; set; }
        public string? DeliveryAddress { get; set; }
    }
}

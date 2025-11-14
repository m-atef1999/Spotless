namespace Spotless.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid? DriverId { get; private set; }

        public int Rating { get; private set; }
        public string? Comment { get; private set; }


        public virtual Customer Customer { get; private set; } = null!;
        public virtual Order Order { get; private set; } = null!;

        protected Review() { }

        public Review(Guid customerId, Guid orderId, int rating, string? comment, Guid? driverId = null) : base()
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
            }

            CustomerId = customerId;
            OrderId = orderId;
            Rating = rating;
            Comment = comment;
            DriverId = driverId;
        }
    }
}

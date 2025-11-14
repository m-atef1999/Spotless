using Spotless.Domain.ValueObjects;

namespace Spotless.Domain.Entities
{
    public class Service : BaseEntity
    {

        public Guid CategoryId { get; init; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public Money BasePrice { get; private set; }

        public Money PricePerUnit { get; private set; } = Money.Zero;
        public decimal EstimatedDurationHours { get; private set; }


        public virtual Category Category { get; private set; } = null!;


        protected Service()
        {

            BasePrice = Money.Zero;

        }

        public Service(
            Guid categoryId,
            string name,
            string description,

            Money pricePerUnit,
            decimal estimatedDurationHours) : base()
        {
            if (estimatedDurationHours <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(estimatedDurationHours), "Duration must be positive.");
            }

            CategoryId = categoryId;
            Name = name;
            Description = description;
            PricePerUnit = pricePerUnit;
            EstimatedDurationHours = estimatedDurationHours;


            BasePrice = Money.Zero;
        }

        public void UpdateDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void UpdatePricingAndDuration(Money pricePerUnit, decimal estimatedDurationHours)
        {
            if (estimatedDurationHours <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(estimatedDurationHours), "Duration must be positive.");
            }
            PricePerUnit = pricePerUnit;
            EstimatedDurationHours = estimatedDurationHours;
        }


        public void SetBasePrice(Money basePrice)
        {
            BasePrice = basePrice;
        }
    }
}

using Spotless.Domain.ValueObjects;

namespace Spotless.Domain.Entities
{
    public class Service : BaseEntity
    {
        public Guid CategoryId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public Money BasePrice { get; private set; } = Money.Zero;
        public Money PricePerUnit { get; private set; } = Money.Zero;
        public decimal EstimatedDurationHours { get; private set; }

        public virtual Category Category { get; private set; } = null!;


        protected Service() { }


        public Service(
            Guid categoryId,
            string name,
            string description,
            Money pricePerUnit,
            decimal estimatedDurationHours) : base()
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;


            PricePerUnit = pricePerUnit;
            EstimatedDurationHours = estimatedDurationHours;
        }

    }
}
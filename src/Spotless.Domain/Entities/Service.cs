using System;
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
        public bool IsActive { get; private set; } = true;
        public bool IsFeatured { get; private set; } = false;
        public decimal MaxWeightKg { get; private set; } = 50m; // Default max weight per service
        
        // Image support - URL for external images only
        public string? ImageUrl { get; private set; }

        public virtual Category Category { get; private set; } = null!;


        protected Service() { }


        public Service(
            Guid categoryId,
            string name,
            string description,
            Money pricePerUnit,
            decimal estimatedDurationHours,
            decimal? maxWeightKg = null,
            string? imageUrl = null) : base()
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;

            PricePerUnit = pricePerUnit;
            BasePrice = pricePerUnit;
            EstimatedDurationHours = estimatedDurationHours;
            ImageUrl = imageUrl;
            if (maxWeightKg.HasValue)
            {
                if (maxWeightKg.Value <= 0) throw new ArgumentException("MaxWeightKg must be positive.", nameof(maxWeightKg));
                MaxWeightKg = maxWeightKg.Value;
            }
        }

        public void Update(
            string? name,
            string? description,
            Money? pricePerUnit,
            decimal? estimatedDurationHours,
            Guid? categoryId,
            decimal? maxWeightKg = null,
            string? imageUrl = null)
        {
            if (maxWeightKg.HasValue && maxWeightKg.Value <= 0)
                throw new ArgumentException("MaxWeightKg must be positive.", nameof(maxWeightKg));

            if (maxWeightKg.HasValue)
                MaxWeightKg = maxWeightKg.Value;

            if (name != null)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Name cannot be empty.", nameof(name));
                Name = name;
            }

            if (description != null)
                Description = description;

            if (pricePerUnit != null)
                PricePerUnit = pricePerUnit;

            if (estimatedDurationHours.HasValue)
            {
                if (estimatedDurationHours.Value <= 0)
                    throw new ArgumentException("Duration must be positive.", nameof(estimatedDurationHours));
                EstimatedDurationHours = estimatedDurationHours.Value;
            }

            if (categoryId.HasValue)
                CategoryId = categoryId.Value;

            // Image can be updated to null (clear) or new value
            if (imageUrl != null)
            {
                ImageUrl = imageUrl;
            }
        }

        public void SetImage(string? imageUrl)
        {
            ImageUrl = imageUrl;
        }
        
        public void SetFeatured(bool isFeatured)
        {
            IsFeatured = isFeatured;
        }
        
        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }

    }
}


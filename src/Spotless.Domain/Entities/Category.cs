using Spotless.Domain.ValueObjects;


namespace Spotless.Domain.Entities
{

    public class Category : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public Money Price { get; private set; } = null!;
        
        // Image support - URL for external images only
        public string? ImageUrl { get; private set; }

        private readonly List<Service> _services = [];
        public IReadOnlyCollection<Service> Services => _services.AsReadOnly();

        protected Category() { }

        public Category(string name, Money price, string? description = null, string? imageUrl = null) : base()
        {
            Name = name;
            Price = price;
            Description = description;
            ImageUrl = imageUrl;
        }

        public void UpdateDetails(string name, Money price, string? description = null, string? imageUrl = null)
        {
            Name = name;
            Price = price;
            Description = description;
            ImageUrl = imageUrl;
        }

        public void SetImage(string? imageUrl)
        {
            ImageUrl = imageUrl;
        }
    }
}


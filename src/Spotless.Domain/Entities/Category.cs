using Spotless.Domain.ValueObjects;


namespace Spotless.Domain.Entities
{

    public class Category : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public Money Price { get; private set; } = null!;
        
        // Image support - URL for external images, Data for uploaded files
        public string? ImageUrl { get; private set; }
        public string? ImageData { get; private set; } // Base64 encoded image data

        private readonly List<Service> _services = [];
        public IReadOnlyCollection<Service> Services => _services.AsReadOnly();

        protected Category() { }

        public Category(string name, Money price, string? description = null, string? imageUrl = null, string? imageData = null) : base()
        {
            Name = name;
            Price = price;
            Description = description;
            ImageUrl = imageUrl;
            ImageData = imageData;
        }

        public void UpdateDetails(string name, Money price, string? description = null, string? imageUrl = null, string? imageData = null)
        {
            Name = name;
            Price = price;
            Description = description;
            ImageUrl = imageUrl;
            ImageData = imageData;
        }

        public void SetImage(string? imageUrl, string? imageData)
        {
            ImageUrl = imageUrl;
            ImageData = imageData;
        }
    }
}


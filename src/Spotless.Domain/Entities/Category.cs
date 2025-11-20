using Spotless.Domain.ValueObjects;


namespace Spotless.Domain.Entities
{

    public class Category : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public Money Price { get; private set; } = null!;

        private readonly List<Service> _services = [];
        public IReadOnlyCollection<Service> Services => _services.AsReadOnly();

        protected Category() { }

        public Category(string name, Money price) : base()
        {
            Name = name;
            Price = price;
        }
    }
}

namespace Spotless.Domain.Entities
{
    public class Service : BaseEntity
    {

        public Guid CategoryId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public virtual Category Category { get; private set; } = null!;

        protected Service() { }

        public Service(Guid categoryId, string name, string description) : base()
        {
            CategoryId = categoryId;
            Name = name;
            Description = description;
        }
    }
}

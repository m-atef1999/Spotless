using Spotless.Domain.Events;

namespace Spotless.Application.Interfaces
{
    public interface IDomainEventPublisher
    {
        Task PublishAsync<T>(T domainEvent) where T : IDomainEvent;
    }
}
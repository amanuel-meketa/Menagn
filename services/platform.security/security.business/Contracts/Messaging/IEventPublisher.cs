using security.data.Entities.Events;

namespace security.business.Contracts.Messaging
{
    public interface IEventPublisher
    {
        Task PublishRoleCreatedAsync( RoleCreatedEvent @event, CancellationToken cancellationToken = default);
    }
}

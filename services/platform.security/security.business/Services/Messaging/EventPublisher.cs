using Confluent.Kafka;
using security.business.Contracts.Messaging;
using security.data.Entities.Events;
using System.Text.Json;

namespace security.business.Services.Messaging
{
    public sealed class KafkaEventPublisher : IEventPublisher
    {
        private const string RoleCreatedTopic = "role.created";
        private readonly IProducer<string, string> _producer;

        public KafkaEventPublisher(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public async Task PublishRoleCreatedAsync(RoleCreatedEvent @event, CancellationToken cancellationToken = default)
        {
            var message = new Message<string, string>
            {
                Key = @event.RoleId.ToString(),
                Value = JsonSerializer.Serialize(@event)
            };

            try
            {
                var deliveryResult = await _producer.ProduceAsync(RoleCreatedTopic, message, cancellationToken);
                Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine($"Kafka produce error: {ex.Error.Reason}");
                throw;
            }
        }
    }
}

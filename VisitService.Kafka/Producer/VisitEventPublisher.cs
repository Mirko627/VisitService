using Microsoft.Extensions.Options;
using VisitService.Kafka.Topics;
using System.Text.Json;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Messages;
using VisitService.Shared.kafka.Events;
using VisitService.Shared.kafka.Contracts;
using VisitService.Repository.Entities;

namespace VisitService.Kafka.Producer
{
    public class VisitEventPublisher : IVisitEventPublisher
    {
        private const string Insert = "I";
        private const string Update = "U";
        private const string Delete = "D";
        
        private readonly VisitServiceTopicsOutput _topics;

        public VisitEventPublisher(
            IOptions<VisitServiceTopicsOutput> topics)
        {
            _topics = topics.Value;
        }

        public OutboxEvent CreateVisitCreatedEvent(VisitCreatedDto visit)
            => CreateEvent(VisitKafkaEvents.VisitCreated, Insert, visit);

        public OutboxEvent CreateVisitConfirmedEvent(VisitConfirmedDto visit)
            => CreateEvent(VisitKafkaEvents.VisitConfirmed, Insert, visit);

        public OutboxEvent CreateVisitRejectedEvent(VisitRejectedDto visit)
            => CreateEvent(VisitKafkaEvents.VisitRejected, Insert, visit);

        public OutboxEvent CreateVisitCompletedEvent(VisitCompletedDto visit)
            => CreateEvent(VisitKafkaEvents.VisitCompleted, Insert, visit);

        private OutboxEvent CreateEvent<T>(string eventType, string operation, T dto)
        {
            var operationMessage = new OperationMessage<T>
            {
                Operation = operation,
                Dto = dto
            };

            string json = JsonSerializer.Serialize(operationMessage);

            return new OutboxEvent
            {
                Id = Guid.NewGuid(),
                EventType = eventType,
                Topic = _topics.VisitEvents,
                Key = eventType,
                Payload = json,
                CreatedAt = DateTime.UtcNow,
                PublishedAt = null
            };
        }
    }
}
using Microsoft.Extensions.Options;
using VisitService.Kafka.Contracts;
using VisitService.Kafka.Events;
using VisitService.Kafka.Topics;
using System.Text.Json;
using Utility.Kafka.Abstractions.Clients;
using Utility.Kafka.Messages;

namespace VisitService.Kafka.Producer
{
    public class VisitEventPublisher : IVisitEventPublisher
    {
        private const string Insert = "I";
        private const string Update = "U";
        private const string Delete = "D";
        
        private readonly IProducerClient<string, string> _producerClient;
        private readonly VisitServiceTopicsOutput _topics;

        public VisitEventPublisher(
            IProducerClient<string, string> producerClient,
            IOptions<VisitServiceTopicsOutput> topics)
        {
            _producerClient = producerClient;
            _topics = topics.Value;
        }

        public Task VisitCreatedAsync(VisitCreatedDto visit)
            => PublishAsync(VisitKafkaEvents.VisitCreated, Insert, visit);

        public Task VisitConfirmedAsync(VisitConfirmedDto visit)
            => PublishAsync(VisitKafkaEvents.VisitConfirmed, Insert, visit);

        public Task VisitRejectedAsync(VisitRejectedDto visit)
            => PublishAsync(VisitKafkaEvents.VisitRejected, Insert, visit);

        public Task VisitCompletedAsync(VisitCompletedDto visit)
            => PublishAsync(VisitKafkaEvents.VisitCompleted, Insert, visit);

        private async Task PublishAsync<T>(string kafkaKey, string crudOperation, T visitDto)
        {
            var operationMessage = new OperationMessage<T>
            {
                Operation = crudOperation,
                Dto = visitDto
            };

            string json = JsonSerializer.Serialize(operationMessage);

            await _producerClient.ProduceAsync(
                _topics.VisitEvents,
                kafkaKey,
                json);
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace VisitService.Kafka.Topics;

public class VisitServiceTopicsOutput : AbstractKafkaTopics
{
    public string VisitEvents { get; set; } = "offer-events";

    public override IEnumerable<string> GetTopics()
    {
        return [VisitEvents];
    }
}
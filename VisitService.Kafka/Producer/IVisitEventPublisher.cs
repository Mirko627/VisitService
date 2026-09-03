using VisitService.Repository.Entities;
using VisitService.Shared.kafka.Contracts;

namespace VisitService.Kafka.Producer
{
    public interface IVisitEventPublisher
    {
        OutboxEvent CreateVisitCreatedEvent(VisitCreatedDto visit);
        OutboxEvent CreateVisitConfirmedEvent(VisitConfirmedDto visit);
        OutboxEvent CreateVisitRejectedEvent(VisitRejectedDto visit);
        OutboxEvent CreateVisitCompletedEvent(VisitCompletedDto visit);
    }
}

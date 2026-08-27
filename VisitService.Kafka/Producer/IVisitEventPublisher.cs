using VisitService.Kafka.Contracts;

namespace VisitService.Kafka.Producer
{
    public interface IVisitEventPublisher
    {
        Task VisitCreatedAsync(VisitCreatedDto visit);
        Task VisitConfirmedAsync(VisitConfirmedDto visit);
        Task VisitRejectedAsync(VisitRejectedDto visit);
        Task VisitCompletedAsync(VisitCompletedDto visit);
    }
}

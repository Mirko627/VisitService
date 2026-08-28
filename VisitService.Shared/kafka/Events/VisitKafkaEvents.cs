//Contiene il nome degli eventi pubblicati da OfferService
namespace VisitService.Shared.kafka.Events;

public static class VisitKafkaEvents
{
    public const string VisitCreated = "VisitCreated";
    public const string VisitConfirmed = "VisitConfirmed";
    public const string VisitRejected = "VisitRejected";
    public const string VisitCompleted = "VisitCompleted";
}
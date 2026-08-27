namespace VisitService.Kafka.Contracts
{
    public class VisitCreatedDto
    {
        public required int PropertyId { get; set; }
        public DateTime VisitDate { get; set; }
        public int VisitatorId { get; set; }
        public int OwnerId { get; set; }
    }
}

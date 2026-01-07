namespace VisitService.Shared.dtos
{
    public class UpdateVisitDto
    {
        public required int PropertyId { get; set; }
        public DateTime VisitDate { get; set; }
    }
}

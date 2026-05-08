namespace VisitService.Shared.dtos
{
    public class CreateVisitDto
    {
        public required int PropertyId { get; set; }
        public DateTime VisitDate { get; set; }
    }
}

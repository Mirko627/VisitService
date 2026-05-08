using System.ComponentModel.DataAnnotations;
using VisitService.Shared.enums;

namespace VisitService.Shared.dtos
{
    public class VisitDto
    {

        public required int Id { get; set; }
        public required int PropertyId { get; set; }
        public VisitStatus Status { get; set; } = VisitStatus.Pending;
        public DateTime VisitDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int VisitatorId { get; set; }
        public int OwnerId { get; set; }

    }
}

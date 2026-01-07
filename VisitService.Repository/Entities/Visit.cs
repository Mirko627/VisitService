using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisitService.Shared.enums;

namespace VisitService.Repository.Entities
{
    public class Visit
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required int Id { get; set; }
        [Required]
        public required int PropertyId { get; set; }
        public VisitStatus Status { get; set; } = VisitStatus.Pending;
        [Required]
        public DateTime VisitDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public int VisitatorId { get; set; }
        public int OwnerId { get; set; }

    }
}

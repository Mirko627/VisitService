using Microsoft.EntityFrameworkCore;
using VisitService.Repository.Entities;
using VisitService.Shared.enums;

namespace VisitService.Data.Context
{
    public class VisitDbContext : DbContext
    {
        public DbSet<Visit> visits { get; set; }

        public VisitDbContext(DbContextOptions<VisitDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Visit>(entity =>
            {
                entity.Property(p => p.Status)
                    .HasConversion<string>()
                    .IsRequired()
                    .HasDefaultValue(VisitStatus.Pending);

                entity.ToTable(t => t.HasCheckConstraint(
                    name: "CK_Visit_Status",
                    sql: "Status IN ('Pending','Confirmed','Cancelled', 'Completed')"));

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

            });
        }
    }     
}

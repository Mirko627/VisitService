using Microsoft.EntityFrameworkCore;
using VisitService.Repository.Entities;
using VisitService.Shared.enums;

namespace VisitService.Data.Context
{
    public class VisitDbContext : DbContext
    {
        public DbSet<Visit> Visits { get; set; }
        public DbSet<OutboxEvent> OutboxEvents { get; set; }

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

            builder.Entity<OutboxEvent>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Topic)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Payload)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.PublishedAt)
                    .IsRequired(false);

                entity.HasIndex(e => new { e.PublishedAt, e.CreatedAt });
            });
        }
    }     
}

using Microsoft.EntityFrameworkCore;
using Roboto.Models;

namespace Roboto.Repository;

public class RobotoDbContext : DbContext
{
    public RobotoDbContext(DbContextOptions<RobotoDbContext> options) : base(options)
    {
    }

    public virtual DbSet<CalendarEvent> CalendarEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CalendarEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.StartDateTime).IsRequired();
            entity.Property(e => e.EndDateTime).IsRequired();
            entity.Property(e => e.Details).HasMaxLength(1000);
        });
    }

}

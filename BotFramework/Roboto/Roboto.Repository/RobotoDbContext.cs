using Microsoft.EntityFrameworkCore;
using Roboto.Models;

namespace Roboto.Repository;

public class RobotoDbContext : DbContext
{
    public RobotoDbContext(DbContextOptions<RobotoDbContext> options) : base(options)
    {
    }

    public virtual DbSet<CalendarEvent> CalendarEvents { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Salt).HasMaxLength(255).IsRequired();
            entity.HasMany(e => e.CalendarEvents)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId);
        });

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

using Microsoft.EntityFrameworkCore;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.EntityFramework.Configurations;

namespace TicketService.Infrastructure.EntityFramework.Contexts;

public class TicketDbContext : DbContext
{
    public TicketDbContext(DbContextOptions<TicketDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<Status> Statuses { get; set; }

    public DbSet<Priority> Priorities { get; set; }

    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<TicketStatusHistory> TicketStatusHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new StatusConfiguration());
        modelBuilder.ApplyConfiguration(new PriorityConfiguration());
        modelBuilder.ApplyConfiguration(new TicketConfiguration());
        modelBuilder.ApplyConfiguration(new TicketStatusHistoryConfiguration());
    }
}

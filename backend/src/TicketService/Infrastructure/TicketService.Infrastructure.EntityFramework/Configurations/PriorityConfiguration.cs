using TicketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketService.Infrastructure.EntityFramework.Configurations;

public class PriorityConfiguration
    : IEntityTypeConfiguration<Priority>
{
    public void Configure(EntityTypeBuilder<Priority> builder)
    {
        builder.ToTable("Priorities");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.Description).IsUnique();
    }
}
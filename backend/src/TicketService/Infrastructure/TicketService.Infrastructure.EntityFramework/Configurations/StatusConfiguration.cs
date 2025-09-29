using TicketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketService.Infrastructure.EntityFramework.Configurations;

public class StatusConfiguration
    : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("Statuses");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.Description).IsUnique();
    }
}
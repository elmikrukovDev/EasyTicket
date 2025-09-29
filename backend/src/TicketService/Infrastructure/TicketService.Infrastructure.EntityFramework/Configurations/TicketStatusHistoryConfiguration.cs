using TicketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TicketService.Infrastructure.EntityFramework.Configurations;

public class TicketStatusHistoryConfiguration
    : IEntityTypeConfiguration<TicketStatusHistory>
{
    public void Configure(EntityTypeBuilder<TicketStatusHistory> builder)
    {
        builder.ToTable("TicketStatusHistory");

        builder.HasKey(tsh => tsh.Id);

        builder.Property(tsh => tsh.Comment)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(tsh => tsh.ChangedAt)
            .IsRequired();

        builder.Property(tsh => tsh.OldStatusId)
            .IsRequired();

        builder.Property(tsh => tsh.NewStatusId)
            .IsRequired();
        
        builder.Property(tsh => tsh.ChangedById)
            .IsRequired();

        builder.HasOne(tsh => tsh.Ticket)
            .WithMany(t => t.TicketStatusHistory)
            .HasForeignKey(t => t.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
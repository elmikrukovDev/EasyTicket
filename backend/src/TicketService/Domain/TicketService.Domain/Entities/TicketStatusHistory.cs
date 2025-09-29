
namespace TicketService.Domain.Entities;

public class TicketStatusHistory : IBaseEntity
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public string? Comment { get; set; }
    public DateTime ChangedAt { get; set; }
    public Guid OldStatusId { get; set; }
    public Guid NewStatusId { get; set; }
    public Guid TicketId { get; set; }
    public Guid ChangedById { get; set; }
    public virtual Ticket Ticket { get; set; } = null!;
}

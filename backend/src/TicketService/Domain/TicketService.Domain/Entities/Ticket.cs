
namespace TicketService.Domain.Entities;

public class Ticket : IBaseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ClosedAt { get; set; }
    public Guid StatusId { get; set; }
    public Guid PriorityId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CreatedById { get; set; }
    public Guid? AssignedToId { get; set; }

    public virtual Status Status { get; set; } = null!;
    public virtual Priority Priority { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<TicketStatusHistory> TicketStatusHistory { get; set; } = [];
}

namespace TicketService.Domain.Entities;

public class Priority : IBaseEntity
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public virtual ICollection<Ticket> Tickets { get; set; } = [];
}

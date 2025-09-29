
namespace TicketService.Domain.Entities;

public class Status : IBaseEntity
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
}

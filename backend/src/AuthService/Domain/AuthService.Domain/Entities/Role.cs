using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public virtual ICollection<User> Users { get; set; } = [];
}
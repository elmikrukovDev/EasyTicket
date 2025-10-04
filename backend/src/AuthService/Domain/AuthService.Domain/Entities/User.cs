using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime LoginAt { get; set; }

    public Guid RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
    public string[] Groups { get; set; }
}
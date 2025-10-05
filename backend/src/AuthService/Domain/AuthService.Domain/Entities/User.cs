using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public new string UserName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime LoginAt { get; set; }
    public string Role { get; set; } = null!;
}
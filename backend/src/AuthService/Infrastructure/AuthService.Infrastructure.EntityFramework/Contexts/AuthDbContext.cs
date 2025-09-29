using AuthService.Domain.Entities;
using AuthService.Infrastructure.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.EntityFramework.Contexts;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}

using AuthService.Domain.Entities;
using System.Security.Claims;

namespace AuthService.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    ClaimsPrincipal ValidateToken(string token);
}

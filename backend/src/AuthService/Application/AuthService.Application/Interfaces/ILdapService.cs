using AuthService.Application.DTOs;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface ILdapService
{
    Task<bool> AuthenticateAsync(LoginRequest request);
    Task<User> GetUserAsync(LoginRequest request);
}

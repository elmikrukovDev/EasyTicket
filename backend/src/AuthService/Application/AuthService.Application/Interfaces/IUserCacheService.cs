using AuthService.Application.DTOs;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface IUserCacheService
{
    Task<User> GetUserAsync(LoginRequest request);
}
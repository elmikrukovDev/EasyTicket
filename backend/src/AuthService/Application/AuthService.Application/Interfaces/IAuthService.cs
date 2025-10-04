using AuthService.Application.DTOs;

namespace AuthService.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponce> LoginAsync(LoginRequest request);
}
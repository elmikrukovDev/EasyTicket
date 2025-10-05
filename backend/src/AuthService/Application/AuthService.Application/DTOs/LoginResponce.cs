namespace AuthService.Application.DTOs;

public class LoginResponce
{
    public string Token { get; set; } = null!;
    public string UserName { get; set; } = null!;
}
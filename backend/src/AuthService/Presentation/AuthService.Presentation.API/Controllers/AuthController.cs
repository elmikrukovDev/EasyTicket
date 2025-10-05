using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Presentation.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ILdapService _ldapService;
    private readonly IUserCacheService _userCache;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ILdapService ldapService,
        IUserCacheService userCache,
        IJwtService jwtService,
        ILogger<AuthController> logger)
    {
        _ldapService = ldapService;
        _userCache = userCache;
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponce>> Login(
        [FromBody] LoginRequest request)
    {
        try
        {
            bool isAuth = await _ldapService.AuthenticateAsync(request);

            if (!isAuth) return Unauthorized("Auth failed");

            User user = await _userCache.GetUserAsync(request);

            var token = _jwtService.GenerateToken(user);

            var responce = new LoginResponce()
            {
                Token = token,
                UserName = user.UserName
            };

            return Ok(responce);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Unauthorized($"Auth failed: {ex.Message}");
        }
    }
}

using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Settings;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AuthService.Application.Services;

public class UserCacheService : IUserCacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILdapService _ldapService;
    private readonly TimeSpan _defaultExpiration;

    public UserCacheService(
        IDistributedCache cache,
        ILdapService ldapService,
        IOptions<RedisSettings> settings)
    {
        _cache = cache;
        _ldapService = ldapService;
        _defaultExpiration = TimeSpan.FromMinutes(settings.Value.DefaultExpiration);
    }

    public async Task<User?> GetUserAsync(LoginRequest request)
    {
        var cacheKey = $"user:{request.UserName.ToLower()}";

        var cachedUser = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedUser))
        {
            return JsonSerializer.Deserialize<User>(cachedUser);
        }

        var user = await _ldapService.GetUserAsync(request);
        if (user != null)
        {
            await SetUserAsync(user);
        }

        return user;
    }

    public async Task SetUserAsync(User user, TimeSpan? expiry = null)
    {
        var cacheKey = $"user:{user.UserName?.ToLower()}";
        var serializedUser = JsonSerializer.Serialize(user);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? _defaultExpiration
        };

        await _cache.SetStringAsync(cacheKey, serializedUser, options);
    }

    public async Task RemoveUserAsync(string samAccountName)
    {
        var cacheKey = $"user:{samAccountName.ToLower()}";
        await _cache.RemoveAsync(cacheKey);
    }
}
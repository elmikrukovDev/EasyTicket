using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Settings;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace AuthService.Application.Services;

public class LdapService : ILdapService
{
    private readonly LdapSettings _settings;
    private readonly ILogger<LdapService> _logger;

    public LdapService(
        IOptions<LdapSettings> settings,
        ILogger<LdapService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<bool> AuthenticateAsync(LoginRequest request)
    {
        using var connection = new LdapConnection();
        connection.SecureSocketLayer = _settings.UseSsl;

        await connection.ConnectAsync(_settings.Server, _settings.Port);

        await connection.BindAsync($"{request.UserName}@{_settings.Domain}", request.Password);

        if (!connection.Bound)
        {
            _logger.LogWarning("Invalid credentials for user: {Username}", request.UserName);
            return false;
        }

        _logger.LogInformation("User {Username} successfully authenticated", request.UserName);
        return true;
    }

    public async Task<User> GetUserAsync(LoginRequest request)
    {
        using var connection = new LdapConnection();
        connection.SecureSocketLayer = _settings.UseSsl;

        await connection.ConnectAsync(_settings.Server, _settings.Port);
        await connection.BindAsync($"{request.UserName}@{_settings.Domain}", request.Password);
        return await GetUserInfoInternalAsync(request.UserName, connection);
    }

    private async Task<User> GetUserInfoInternalAsync(string username, ILdapConnection connection)
    {
        var searchFilter = $"(sAMAccountName={EscapeLdapFilter(username)})";
        var searchResults = await connection.SearchAsync(
                _settings.SearchBase,
                LdapConnection.ScopeSub,
                searchFilter,
                _settings.UserInfoAttributes,
                false
            );

        if (!await searchResults.HasMoreAsync())
            throw new InvalidOperationException(
                "User not found");

        var entry = await searchResults.NextAsync();
        var etGroups = GetUserGroups(entry, connection)
            .Where(_settings.GroupRoleMapping.ContainsKey)
            .Select(g => _settings.GroupRoleMapping[g])
            .ToList();

        if (etGroups.Count != 1)
            throw new InvalidOperationException(
                "Пользователь не имеет роль ET-Groups или " +
                "имеет несколько ролей, что запрещено!");

        return new User
        {
            Id = new Guid(entry.Get("objectGUID").ByteValue),
            UserName = GetAttributeValue(entry, "sAMAccountName") ?? username,
            Email = GetAttributeValue(entry, "mail"),
            DisplayName = GetAttributeValue(entry, "displayName") ??
                $"{GetAttributeValue(entry, "givenName")} " +
                $"{GetAttributeValue(entry, "sn")}".Trim(),
            PhoneNumber = GetAttributeValue(entry, "telephoneNumber"),
            Role = etGroups.First(),
            LoginAt = DateTime.UtcNow
        };
    }

    private string[] GetUserGroups(LdapEntry userEntry, ILdapConnection connection)
    {
        var memberOf = userEntry.GetOrDefault("memberOf");
        if (memberOf == null)
            return [];
        
        var groups = new List<string>();
        foreach (string groupDn in memberOf.StringValueArray)
        {
            var groupName = ExtractCnFromDn(groupDn);
            if (!string.IsNullOrEmpty(groupName))
                groups.Add(groupName);
        }

        return [.. groups];
    }

    private string? GetAttributeValue(LdapEntry entry, string attributeName)
    {
        return entry.GetStringValueOrDefault(attributeName);
    }

    private string? ExtractCnFromDn(string dn)
    {
        if (string.IsNullOrEmpty(dn)) return null;

        var parts = dn.Split(',');
        foreach (var part in parts)
        {
            if (part.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                return part.Substring(3).Trim();
        }

        return null;
    }

    private string EscapeLdapFilter(string filter)
    {
        if (string.IsNullOrEmpty(filter)) return filter;

        return filter
            .Replace("\\", "\\5c")
            .Replace("*", "\\2a")
            .Replace("(", "\\28")
            .Replace(")", "\\29")
            .Replace("\0", "\\00");
    }
}
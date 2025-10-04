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
        try
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
        catch (LdapException ex)
        {
            _logger.LogError(ex, "LDAP authentication failed for user {Username}. Error: {ErrorCode}", request.UserName, ex.ResultCode);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user {Username}", request.UserName);
            throw;
        }
    }

    public async Task<User?> GetUserAsync(LoginRequest request)
    {
        try
        {
            using var connection = new LdapConnection();
            connection.SecureSocketLayer = _settings.UseSsl;

            await connection.ConnectAsync(_settings.Server, _settings.Port);
            await connection.BindAsync($"{request.UserName}@{_settings.Domain}", request.Password);
            return await GetUserInfoInternalAsync(request.UserName, connection);
        }
        catch (LdapException ex)
        {
            _logger.LogError(ex, "LDAP authentication failed for user {Username}. Error: {ErrorCode}", request.UserName, ex.ResultCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user {Username}", request.UserName);
            throw;
        }
    }

    private async Task<User?> GetUserInfoInternalAsync(string username, ILdapConnection connection)
    {
        var searchFilter = $"(sAMAccountName={EscapeLdapFilter(username)})";
        var attributes = new[] {
            "sAMAccountName",
            "mail",
            "telephoneNumber",
            "displayName",
            "memberOf",
            "cn",
            "ou",
            "title",
            "department",
            "sn",
            "givenName",
            "objectGUID",
            "uid"
        };

        var searchResults = await connection.SearchAsync(
                _settings.SearchBase,
                LdapConnection.ScopeSub,
                searchFilter,
                attributes,
                false
            );

        if (!await searchResults.HasMoreAsync())
        {
            return null;
        }

        var entry = await searchResults.NextAsync();
        var groups = GetUserGroups(entry, connection);

        return new User
        {
            Id = new Guid(entry.Get("objectGUID").ByteValue),
            UserName = GetAttributeValue(entry, "sAMAccountName") ?? username,
            Email = GetAttributeValue(entry, "mail"),
            DisplayName = GetAttributeValue(entry, "displayName") ??
                         $"{GetAttributeValue(entry, "givenName")} {GetAttributeValue(entry, "sn")}".Trim(),
            PhoneNumber = GetAttributeValue(entry, "telephoneNumber"),
            Groups = groups
        };
    }

    private string[] GetUserGroups(LdapEntry userEntry, ILdapConnection connection)
    {
        var groups = new List<string>();

        var memberOf = userEntry.Get("memberOf");
        if (memberOf != null)
        {
            foreach (string groupDn in memberOf.StringValueArray)
            {
                var groupName = ExtractCnFromDn(groupDn);
                if (!string.IsNullOrEmpty(groupName))
                {
                    groups.Add(groupName);
                }
            }
        }

        return groups.ToArray();
    }

    private string GetAttributeValue(LdapEntry entry, string attributeName)
    {
        var attribute = entry.Get(attributeName);
        return attribute?.StringValue ?? string.Empty;
    }

    private string? ExtractCnFromDn(string dn)
    {
        if (string.IsNullOrEmpty(dn)) return null;

        var parts = dn.Split(',');
        foreach (var part in parts)
        {
            if (part.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
            {
                return part.Substring(3).Trim();
            }
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
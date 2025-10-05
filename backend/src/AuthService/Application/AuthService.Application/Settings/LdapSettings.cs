namespace AuthService.Application.Settings;

public class LdapSettings
{
    public string Server { get; set; } = null!;
    public int Port { get; set; } = 389;
    public bool UseSsl { get; set; } = false;
    public string SearchBase { get; set; } = null!;
    public string Domain { get; set; } = null!;
    public string[] UserInfoAttributes { get; set; } = null!;
    public Dictionary<string, string> GroupRoleMapping { get; set; } = new();
}
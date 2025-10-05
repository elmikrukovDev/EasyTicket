namespace AuthService.Application.Settings;

public class RedisSettings
{
    public string ConnectionString { get; set; } = null!;
    public int DefaultExpiration { get; set; }
}
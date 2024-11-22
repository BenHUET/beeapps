namespace BeeApps.Common.WebAPI.Options;

public class TokenOptions
{
    public string Key { get; set; }
    public int AccessLifetimeInMinutes { get; set; }
    public int RefreshLifetimeInMinutes { get; set; }
}
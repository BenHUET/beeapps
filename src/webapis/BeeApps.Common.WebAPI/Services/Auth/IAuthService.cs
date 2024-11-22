namespace BeeApps.Common.Services;

public interface IAuthService
{
    public Task<bool> Authorize(string token);
}
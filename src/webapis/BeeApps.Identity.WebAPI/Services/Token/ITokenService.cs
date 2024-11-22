using BeeApps.Common.Models;

namespace BeeApps.Common.Services;

public interface ITokenService
{
    public Task<Token> Generate(User user, int lifetimeInMinutes);
    public Task<bool> Validate(string token);
    public Task<int> GetUserId(string token);
}
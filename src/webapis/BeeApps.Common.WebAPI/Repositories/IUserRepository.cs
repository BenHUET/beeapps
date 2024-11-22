using BeeApps.Common.Models;

namespace BeeApps.Common.Repositories;

public interface IUserRepository : ICRUDRepository<User>
{
    public Task<User> GetByToken(string token);
    public Task<User> GetByEmail(string email);

    public Task<bool> IsUniqueUsername(string username);
    public Task<bool> IsUniqueEmail(string email);
}
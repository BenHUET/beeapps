using BeeApps.Common.Contexts;
using BeeApps.Common.Models;
using BeeApps.Common.WebAPI.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BeeApps.Common.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserContext _userContext;

    public UserRepository(UserContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<User> GetById(int id)
    {
        var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            throw new NoDataFoundException();

        return user;
    }

    public async Task Insert(User user)
    {
        await _userContext.Users.AddAsync(user);
        await _userContext.SaveChangesAsync();
    }

    public async Task Update(User user)
    {
        _userContext.Attach(user);
        await _userContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> List()
    {
        return _userContext.Users;
    }

    public async Task<User> Delete()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetByToken(string token)
    {
        var user = await _userContext
            .Users
            .FirstOrDefaultAsync(u => u.ValidationToken == token);

        if (user == null)
            throw new NoDataFoundException();

        return user;
    }

    public async Task<User> GetByEmail(string email)
    {
        var user = await _userContext
            .Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user == null)
            throw new NoDataFoundException();

        return user;
    }

    public async Task<bool> IsUniqueUsername(string username)
    {
        return !await _userContext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<bool> IsUniqueEmail(string email)
    {
        return !await _userContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
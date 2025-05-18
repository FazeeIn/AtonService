using System.Security.Authentication;
using Aton.Core.Exceptions;
using Aton.DataBase.Abstractions;
using Aton.DataBase.Entity;
using Aton.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aton.DataBase.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataBaseContext _context;

    public UserRepository(DataBaseContext context)
    {
        _context = context;
    }

    public async Task<UserEntity> GetUserById(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        
        return user ?? throw new NotFoundException("User not found");
    }

    public async Task CreateUser(UserEntity user)
    {
        if (!_context.Users.Any(x => x.Login == user.CreatedBy && x.Admin == true))
        {
            throw new AuthenticationException("Access denied");
        }
        
        if (_context.Users.Any(x => x.Login == user.Login))
        {
            throw new ConflictException("User already exists");
        }

        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateIdentityUser(UserIdentityEntity user)
    {
        if (!_context.Users.Any(x => x.Id == user.Id))
        {
            throw new NotFoundException("User not found");
        }

        var userEntity = await _context.Users.FirstAsync(x => x.Id == user.Id);

        userEntity.Login = user.Login ?? userEntity.Login;
        userEntity.Password = user.Password ?? userEntity.Password;
        userEntity.Name = user.Name ?? userEntity.Name;
        userEntity.Gender = user.Gender ?? userEntity.Gender;
        userEntity.Birthday = user.Birthday ?? userEntity.Birthday;
        userEntity.ModifiedOn = DateTime.UtcNow;
        userEntity.ModifiedBy = user.AdminLogin;
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserEntity>> GetActiveUsers()
    {
        return await _context.Users
            .Where(x => x.RevokedOn == null)
            .OrderBy(x => x.CreatedOn)
            .Take(10000000)
            .ToListAsync();
    }

    public async Task<UserEntity> GetUserByLogin(string login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
        
        return user ?? throw new NotFoundException("User not found");
    }

    public async Task<UserEntity?> TryGetUserByLogin(string login)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Login == login);
    }

    public async Task<UserEntity> GetUser(string login, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Login == login && x.Password == password);
        
        return user ?? throw new NotFoundException("User not found");
    }

    public Task<List<UserEntity>> GetUsersOlderThan(int age)
    {
        return  _context.Users
            .Where(x => DateTime.UtcNow.Year - x.Birthday!.Value.Year > age) 
            .Take(10000000)
            .ToListAsync();
    }

    public async Task DeleteUserByLogin(string adminLogin, string userLogin, bool isHardDelete)
    {
        var userEntity = await _context.Users.
            FirstOrDefaultAsync(x => x.Login == userLogin);
        
        if (userEntity == null)
        {
            throw new NotFoundException("User not found");
        }
        
        if (isHardDelete)
        { 
            _context.Users.Remove(userEntity);
        }
        else
        {
            userEntity.RevokedOn = DateTime.UtcNow;
            userEntity.RevokedBy = adminLogin;
        }

        await _context.SaveChangesAsync();
    }

    public async Task ReviveUser(Guid userId, string adminLogin)
    {
        var userEntity = await _context.Users
            .FirstAsync(x => x.Id == userId);
        
        userEntity.RevokedOn = null;
        userEntity.RevokedBy = null;
        userEntity.ModifiedBy = adminLogin;
        userEntity.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}
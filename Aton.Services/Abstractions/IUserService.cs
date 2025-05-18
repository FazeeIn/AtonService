using Aton.Core.Models;
using Aton.DataBase.Entity;

namespace Aton.Services.Abstractions;

public interface IUserService
{
    public Task CreateUser(User user);
    public Task UpdateIdentityUser(UpdateUser updateUser);
    public Task<List<User>> GetActiveUsers(Guid adminId);
    public Task<AboutUserInfo> GetUserByLogin(Guid adminId, string userLogin);
    public Task<User> GetUser(string login, string password);
    public Task<List<User>> GetUsersOlderThan(Guid adminId, int age);
    public Task DeleteUserByLogin(Guid adminId, string userLogin, bool isHardDelete);
    public Task ReviveUser(Guid adminId, Guid userId);
}
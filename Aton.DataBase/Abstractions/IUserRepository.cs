using Aton.DataBase.Entity;

namespace Aton.DataBase.Abstractions;

public interface IUserRepository
{
    public Task<UserEntity> GetUserById(Guid id);
    public Task CreateUser(UserEntity user);
    public Task UpdateIdentityUser(UserIdentityEntity user);
    public Task<List<UserEntity>> GetActiveUsers();
    public Task<UserEntity> GetUserByLogin(string login);
    public Task<UserEntity?> TryGetUserByLogin(string login);
    public Task<UserEntity> GetUser(string login, string password);
    public Task<List<UserEntity>> GetUsersOlderThan(int age);
    public Task DeleteUserByLogin(string adminLogin, string userLogin, bool isHardDelete);
    public Task ReviveUser(Guid userId, string adminLogin);
}
using Aton.Core.Models;
using Aton.DataBase.Abstractions;
using Aton.DataBase.Entity;
using Aton.Services.Abstractions;
using Aton.Services.Exceptions;

namespace Aton.Services.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task CreateUser(User user)
    {
        var userEntity = new UserEntity
        {
            Id = user.Id,
            Login = user.Login,
            Name = user.Name,
            Gender = user.Gender,
            Birthday = user.Birthday,
            CreatedOn = user.CreatedOn,
            CreatedBy = user.CreatedBy
        };
        
        await _userRepository.CreateUser(userEntity);
    }

    public async Task UpdateIdentityUser(UpdateUser updateUser)
    { 
        var anotherUserTask = _userRepository
            .TryGetUserByLogin(updateUser.Login);
        
        var anotherUserEntity = await anotherUserTask;
        
        if (anotherUserEntity != null)
        {
            throw new ConflictException("User already exists");
        }

        UserIdentityEntity? userIdentityEntity;
        
        if (updateUser.AdminId != null)
        {
            var adminEntity = await ValidateAdminAsync(updateUser.AdminId.Value);
            
            if (adminEntity.RevokedOn != null)
            {
                throw new ConflictException("Admin was revoked");
            }
            
            userIdentityEntity = new UserIdentityEntity
            {
                Id = updateUser.Id,
                Login = updateUser.Login,
                Password = updateUser.Password,
                Name = updateUser.Name,
                Gender = updateUser.Gender,
                Birthday = updateUser.Birthday,
                AdminLogin = adminEntity.Login
            };
            
            await _userRepository.UpdateIdentityUser(userIdentityEntity);
            
            return;
        }
        
        userIdentityEntity = new UserIdentityEntity
        {
            Id = updateUser.Id,
            Login = updateUser.Login,
            Password = updateUser.Password,
            Name = updateUser.Name,
            Gender = updateUser.Gender,
            Birthday = updateUser.Birthday,
            AdminLogin = null
        };

        await _userRepository.UpdateIdentityUser(userIdentityEntity);
    }

    public async Task<List<User>> GetActiveUsers(Guid adminId)
    {
        await ValidateAdminAsync(adminId);
        
        var activeUsers = await _userRepository.GetActiveUsers();

        return activeUsers
            .Select(x => new User(
                x.Id,
                x.Login,
                x.Password,
                x.Name,
                x.Gender,
                x.Birthday,
                x.Admin,
                x.CreatedOn,
                x.CreatedBy))
            .ToList();
    }

    public async Task<AboutUserInfo> GetUserByLogin(Guid adminId, string userLogin)
    {
        await ValidateAdminAsync(adminId);

        var userEntity = await _userRepository.GetUserByLogin(userLogin);
        
        return new AboutUserInfo(
            userEntity.Name,
            userEntity.Gender,
            userEntity.Birthday,
            userEntity.RevokedOn != null);
    }

    public async Task<User> GetUser(string login, string password)
    {
        var userEntity = await _userRepository.GetUser(login, password);
        
        if (userEntity.RevokedOn != null)
        {
            throw new ConflictException("User was revoked");
        }
        
        return new User(
            userEntity.Id,
            userEntity.Login,
            userEntity.Password,
            userEntity.Name,
            userEntity.Gender,
            userEntity.Birthday,
            userEntity.Admin,
            userEntity.CreatedOn,
            userEntity.CreatedBy);
    }

    public async Task<List<User>> GetUsersOlderThan(Guid adminId, int age)
    {
        await ValidateAdminAsync(adminId);
        
        var userEntities = await _userRepository
            .GetUsersOlderThan(age);
        
        return userEntities.Select(x => new User(
                x.Id,
                x.Login,
                x.Password,
                x.Name,
                x.Gender,
                x.Birthday,
                x.Admin,
                x.CreatedOn,
                x.CreatedBy))
            .ToList();
    }

    public async Task DeleteUserByLogin(Guid adminId, string userLogin, bool isHardDelete)
    {
        var adminEntity = await ValidateAdminAsync(adminId);
        
        await _userRepository.DeleteUserByLogin(adminEntity.Login, userLogin, isHardDelete);
    }

    public async Task ReviveUser(Guid adminId, Guid userId)
    {
        var adminEntity = await ValidateAdminAsync(adminId);

        await _userRepository.ReviveUser(userId, adminEntity.Login);
    }
    private async Task<UserEntity> ValidateAdminAsync(Guid id)
    {
        var adminEntity = await _userRepository.GetUserById(id);
        
        return adminEntity.Admin ? adminEntity : throw new ConflictException("Access denied");
    }
}
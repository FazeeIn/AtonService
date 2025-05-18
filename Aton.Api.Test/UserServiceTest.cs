using Aton.Core.Models;
using Aton.DataBase.Abstractions;
using Aton.DataBase.Entity;
using Aton.Services.Exceptions;
using Aton.Services.Services;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Aton.Api.Tests;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    
    public UserServiceTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    private static UpdateUser GetUpdateUser()
    {
        return new UpdateUser(
            Guid.NewGuid(), 
            "userLogin",
            "userPassword",
            "user",
            0,
            DateTime.UtcNow, 
            Guid.NewGuid());
    }
    
    [Fact]
    public async Task UpdateIdentityUser_UserAndAnotherUserNotEqual()
    {
        var updateUser = GetUpdateUser();
        
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(updateUser.Id))
            .ReturnsAsync(new UserEntity { Id = Guid.NewGuid() });
        
        _userRepositoryMock
            .Setup(x => x.GetUserById(updateUser.AdminId!.Value))
            .ReturnsAsync(new UserEntity());
        
        _userRepositoryMock
            .Setup(x => x.GetUserByLogin(updateUser.Login))
            .ReturnsAsync(new UserEntity { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<ConflictException>(() => 
            userService.UpdateIdentityUser(updateUser));
        
        _userRepositoryMock.Verify(x => x.UpdateIdentityUser(It.IsAny<UserIdentityEntity>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateIdentityUser_AdminIsRevoked()
    {
        var updateUser = GetUpdateUser();
        
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid()
        };

        var adminEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            RevokedOn = new DateTime(2010, 1, 1)
        };
            
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(updateUser.Id))
            .ReturnsAsync(userEntity);
        
        _userRepositoryMock
            .Setup(x => x.GetUserById(updateUser.AdminId!.Value))
            .ReturnsAsync(adminEntity);
        
        _userRepositoryMock
            .Setup(x => x.GetUserByLogin(updateUser.Login))
            .ReturnsAsync(userEntity);

        await Assert.ThrowsAsync<ConflictException>(() => 
            userService.UpdateIdentityUser(updateUser));
        
        _userRepositoryMock.Verify(x => x.UpdateIdentityUser(It.IsAny<UserIdentityEntity>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateIdentityUser_AdminIsNotAdmin()
    {
        var updateUser = GetUpdateUser();
        
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid()
        };

        var adminEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            RevokedOn = new DateTime(2010, 1, 1),
            Admin = false
        };
            
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(updateUser.Id))
            .ReturnsAsync(userEntity);
        
        _userRepositoryMock
            .Setup(x => x.GetUserById(updateUser.AdminId!.Value))
            .ReturnsAsync(adminEntity);
        
        _userRepositoryMock
            .Setup(x => x.GetUserByLogin(updateUser.Login))
            .ReturnsAsync(userEntity);

        await Assert.ThrowsAsync<ConflictException>(() => 
            userService.UpdateIdentityUser(updateUser));
        
        _userRepositoryMock.Verify(x => x.UpdateIdentityUser(It.IsAny<UserIdentityEntity>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateIdentityUser_SuccessTest()
    {
        var updateUser = GetUpdateUser();
        
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid()
        };

        var adminEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            RevokedOn = null,
            Admin = true
        };
            
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(updateUser.Id))
            .ReturnsAsync(userEntity);
        
        _userRepositoryMock
            .Setup(x => x.GetUserById(updateUser.AdminId!.Value))
            .ReturnsAsync(adminEntity);
        
        _userRepositoryMock
            .Setup(x => x.GetUserByLogin(updateUser.Login))
            .ReturnsAsync(userEntity);

        await userService.UpdateIdentityUser(updateUser);
        
        _userRepositoryMock.Verify(x => x.UpdateIdentityUser(It.IsAny<UserIdentityEntity>()), Times.Once);
    }
    
    [Fact]
    public async Task GetActiveUsers_AdminNotFound()
    {
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception());

        await Assert.ThrowsAsync<Exception>(() => 
            userService.GetActiveUsers(Guid.NewGuid()));
        
        _userRepositoryMock.Verify(x => x.GetActiveUsers(), Times.Never);
    }
    
    [Fact]
    public async Task GetActiveUsers_AdminNotAdmin()
    {
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(new UserEntity { Id = Guid.NewGuid(), Admin = false });

        await Assert.ThrowsAsync<ConflictException>(() => 
            userService.GetActiveUsers(Guid.NewGuid()));
        
        _userRepositoryMock.Verify(x => x.GetActiveUsers(), Times.Never);
    }
    
    [Fact]
    public async Task GetActiveUsers_SuccessTest()
    {
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(new UserEntity { Id = Guid.NewGuid(), Admin = true });
        
        _userRepositoryMock
            .Setup(x => x.GetActiveUsers())
            .ReturnsAsync([]);

        var result = await userService.GetActiveUsers(Guid.NewGuid());
        
        Assert.Equal(result, result.OrderBy(x => x.CreatedOn));
        
        _userRepositoryMock.Verify(x => x.GetActiveUsers(), Times.Once);
    }
    
    [Fact]
    public async Task GetUserByLogin_SuccessTest()
    {
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(new UserEntity{Id = Guid.NewGuid(), Admin = true });
        
        _userRepositoryMock
            .Setup(x => x.GetUserByLogin(It.IsAny<string>()))
            .ReturnsAsync(new UserEntity
                {
                    Name = "AbsoluteUser", 
                    Gender = 0, 
                    Birthday = null, 
                    RevokedOn = null
                });

        var result = await userService.GetUserByLogin(Guid.NewGuid(), "AbsoluteUser");
        
        Assert.NotNull(result);
        
        _userRepositoryMock.Verify(x => x.GetUserByLogin("AbsoluteUser"), Times.Once);
    }
    
    [Fact]
    public async Task GetUserByLogin_AdminNotAdmin()
    {
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(new UserEntity{Id = Guid.NewGuid(), Admin = false });
        
        _userRepositoryMock
            .Setup(x => x.GetUserByLogin(It.IsAny<string>()))
            .ReturnsAsync(new UserEntity
            {
                Name = "AbsoluteUser", 
                Gender = 0, 
                Birthday = null, 
                RevokedOn = null
            });

        await Assert.ThrowsAsync<ConflictException>(() => 
            userService.GetUserByLogin(Guid.NewGuid(),"someUserLogin"));
        
        _userRepositoryMock.Verify(x => x.GetUserByLogin("someUserLogin"), Times.Never);
    }
    
    [Fact]
    public async Task GetUser_SuccessTest()
    {
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUser(It.IsAny<string>(),It.IsAny<string>()))
            .ReturnsAsync(new UserEntity{ Id = Guid.NewGuid(), Admin = false, RevokedOn = null });

        var result = await userService.GetUser("SomeLogin", "SomePassword");
        
        Assert.NotNull(result);
        
        _userRepositoryMock.Verify(x => x.GetUser("SomeLogin", "SomePassword"), Times.Once);
    }
    
    [Fact]
    public async Task GetUser_UserRevokedOn()
    {
        var userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock
            .Setup(x => x.GetUser(It.IsAny<string>(),It.IsAny<string>()))
            .ReturnsAsync(new UserEntity{Id = Guid.NewGuid(), Admin = false, RevokedOn = DateTime.UtcNow });

        await Assert.ThrowsAsync<ConflictException>(() => userService.GetUser("SomeLogin", "SomePassword"));
        
        _userRepositoryMock.Verify(x => x.GetUser("SomeLogin", "SomePassword"), Times.Once);
    }
    
    [Fact]
    public async Task GetUsersOlderThan_Success()
    {
        var userService = new UserService(_userRepositoryMock.Object);
        
        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(new UserEntity{Id = Guid.NewGuid(), Admin = true});
        
        _userRepositoryMock
            .Setup(x => x.GetUsersOlderThan(It.IsAny<int>()))
            .ReturnsAsync(
                [
                    new UserEntity { Id = Guid.NewGuid(), Birthday = new DateTime(2000,1,1)},
                    new UserEntity { Id = Guid.NewGuid(), Birthday = new DateTime(2001,1,1)},
                    new UserEntity { Id = Guid.NewGuid(), Birthday = new DateTime(2002,1,1)},
                    new UserEntity { Id = Guid.NewGuid(), Birthday = new DateTime(2004,1,1)},
                    new UserEntity { Id = Guid.NewGuid(), Birthday = new DateTime(2007,1,1)},
                ]);

        var result = await userService.GetUsersOlderThan(Guid.NewGuid(),10);
        
        _userRepositoryMock.Verify(x => x.GetUsersOlderThan(10), Times.Once);
        
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task DeleteUserByLogin_Success()
    {
        var userService = new UserService(_userRepositoryMock.Object);
        
        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(new UserEntity{Id = Guid.NewGuid(),Login = "AdminLogin",Admin = true});
        
        _userRepositoryMock
            .Setup(x => x.DeleteUserByLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        await userService.DeleteUserByLogin(Guid.NewGuid(),"userLogin", true);
        
        _userRepositoryMock.Verify(r =>
                r.DeleteUserByLogin("AdminLogin", "userLogin", 
                    true), Times.Once);
    }
    
    [Fact]
    public async Task ReviveUser_Success()
    {
        var userService = new UserService(_userRepositoryMock.Object);
        
        _userRepositoryMock
            .Setup(x => x.GetUserById(It.IsAny<Guid>()))
            .ReturnsAsync(new UserEntity{Id = Guid.NewGuid(),Login = "AdminLogin",Admin = true});
        
        _userRepositoryMock
            .Setup(x => x.ReviveUser(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        await userService.DeleteUserByLogin(Guid.NewGuid(),"userLogin", true);
        
        _userRepositoryMock.Verify(r =>
            r.DeleteUserByLogin("AdminLogin", "userLogin", 
                true), Times.Once);
    }
    
    
}
using Aton.Api.Contracts.Request;
using Aton.Api.Controllers;
using Aton.Core.Models;
using Aton.Services.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace Aton.Api.Tests;

public class UserControlerTests
{
    private readonly Mock<IValidator<User>> _userValidatorMock;
    private readonly Mock<IValidator<UpdateUser>> _updateUserValidatorMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _usercontroller;
    
    public UserControlerTests()
    {
        _userValidatorMock = new Mock<IValidator<User>>();
        _updateUserValidatorMock = new Mock<IValidator<UpdateUser>>();
        _userServiceMock = new Mock<IUserService>();
        _usercontroller = new UserController(
            _userServiceMock.Object, 
            _userValidatorMock.Object, 
            _updateUserValidatorMock.Object);

    }
    
    [Fact]
    public async Task CreateUserTest_ValidUserRequest()
    {
        var userRequest = new CreateUserRequest(
            "FazeeIn",
            "19216811",
            "Daniil",
            0,
            DateTime.UtcNow, 
            true,
            "");
        
        _userValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<User>(), default))
            .ReturnsAsync(new ValidationResult());
        
        var result = await _usercontroller.Create(userRequest);
        
        Assert.IsType<OkResult>(result);
        
        _userServiceMock.Verify(s => s.CreateUser(It.IsAny<User>()), Times.Once);
        
    }
    
    [Fact]
    public async Task CreateUserTest_InvalidUserRequest()
    {
        var userRequest = new CreateUserRequest(
            "hehe",
            "19216811",
            "Daniil",
            0,
            DateTime.UtcNow, 
            true,
            "");
        
        _userValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<User>(), default))
            .ReturnsAsync(new ValidationResult() {Errors = [new ValidationFailure("Name","BadName")]});
        
        var result = await _usercontroller.Create(userRequest);
        
        var badRequestResult  = Assert.IsType<BadRequestObjectResult>(result);
        
        var errors = Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
        
        Assert.Contains(errors, e => e.PropertyName == "Name" && e.ErrorMessage == "BadName");
        
        _userServiceMock.Verify(s => s.CreateUser(It.IsAny<User>()), Times.Never);
    }
}
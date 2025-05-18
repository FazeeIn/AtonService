using Aton.Api.Contracts.Request;
using Aton.Core.Models;
using Aton.Services.Abstractions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Aton.Api.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class UserController: Controller
{
    private readonly IUserService _userService;
    private readonly IValidator<User> _userValidator;
    private readonly IValidator<UpdateUser> _updateUserValidator;
    public UserController(IUserService userService, IValidator<User> userValidator, IValidator<UpdateUser> updateUserValidator)
    {
        _userService = userService;
        _userValidator = userValidator;
        _updateUserValidator = updateUserValidator;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var userId = Guid.NewGuid();

        var user = new User(
            userId,
            request.Login,
            request.Password,
            request.Name,
            request.Gender,
            request.Birthday,
            request.Admin,
            DateTime.UtcNow, 
            request.CreatedBy);
        
        var validatorResult = await _userValidator.ValidateAsync(user);
    
        if (!validatorResult.IsValid)
        {
            return BadRequest(validatorResult.Errors);
        }

        await _userService.CreateUser(user);
        
        return Ok();
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> UpdateIdentityUser(UpdateUserRequest request)
    {
        var updateUser = new UpdateUser(
            request.Id,
            request.Login,
            request.Password,
            request.Name,
            request.Gender,
            request.Birthday,
            request.AdminId);

        var validatorResult = await _updateUserValidator.ValidateAsync(updateUser);
        
        if (!validatorResult.IsValid)
        {
            return BadRequest(validatorResult.Errors);
        }
        
        await _userService.UpdateIdentityUser(updateUser);

        return Ok();
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetActiveUsers(Guid adminId)
    {
        var activeUsers = await _userService.GetActiveUsers(adminId);

        return Ok(activeUsers);
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetUserByLogin(Guid adminId, string userLogin)
    {
        var userInfo = await _userService.GetUserByLogin(adminId, userLogin);

        return Ok(userInfo);
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetUser(string login, string password)
    {
        var user = await _userService.GetUser(login, password);

        return Ok(user);
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetUsersFilteredByAge(Guid adminId, int age)
    {
        var users = await _userService.GetUsersOlderThan(adminId, age);

        return Ok(users);
    }
    [HttpDelete("[action]")]
    public async Task<IActionResult> DeleteUserByLogin(Guid adminId, string userLogin, bool isHardDelete)
    {
        await _userService.DeleteUserByLogin(adminId, userLogin, isHardDelete);
        
        return Ok();
    }
    [HttpPut("[action]")]
    public async Task<IActionResult> ReviveUser(Guid adminId, Guid userId)
    {
        await _userService.ReviveUser(adminId, userId);

        return Ok();
    }
}
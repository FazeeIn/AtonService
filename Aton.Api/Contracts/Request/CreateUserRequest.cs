namespace Aton.Api.Contracts.Request;

public record CreateUserRequest(
    string Login,
    string Password,
    string Name,
    int Gender,
    DateTime? Birthday,
    bool Admin,
    string CreatedBy);
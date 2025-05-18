namespace Aton.Api.Contracts.Request;

public record UpdateUserRequest(
    Guid Id,
    string Login,
    string Password,
    string? Name,
    int? Gender,
    DateTime? Birthday,
    Guid? AdminId);

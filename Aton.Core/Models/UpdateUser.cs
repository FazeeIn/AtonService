namespace Aton.Core.Models;

public class UpdateUser
{
    public Guid Id { get; }
    public string Login { get; }
    public string Password { get; }
    public string? Name { get; }
    public int? Gender { get; }
    public DateTime? Birthday { get; }
    public Guid? AdminId { get; }

    public UpdateUser(Guid id,string login, string password, string? name, int? gender, DateTime? birthday, Guid? adminId)
    {
        Id = id;
        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        Birthday = birthday;
        AdminId = adminId;
    }
}
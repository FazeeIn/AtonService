namespace Aton.Core.Models;

public class User
{
    public Guid Id { get; }
    public string Login { get; }
    public string Password { get; }
    public string Name { get; }
    public int Gender { get; }
    public DateTime? Birthday { get; }
    public bool Admin { get; }
    public DateTime CreatedOn { get; }
    public string CreatedBy { get; }
    
    public User(Guid id, string login, string password, string name, int gender, 
        DateTime? birthday, bool admin, DateTime createdOn, string createdBy)
    {
        Id = id;
        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        Birthday = birthday;
        Admin = admin;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
    }
}
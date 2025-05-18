namespace Aton.DataBase.Entity;

public class UserEntity
{
    public UserEntity()
    {
    }
    
    public UserEntity(Guid id, string login, string password, string name, int gender, 
        DateTime? birthday, bool admin, DateTime createdOn, string createdBy, 
        DateTime? modifiedOn, string? modifiedBy, DateTime? revokedOn, string? revokedBy)
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
        ModifiedOn = modifiedOn;
        ModifiedBy = modifiedBy;
        RevokedOn = revokedOn;
        RevokedBy = revokedBy;
    }

    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get;  set;}
    public int Gender { get;  set;}
    public DateTime? Birthday { get;  set;}
    public bool Admin { get;  set;}
    public DateTime CreatedOn { get;  set;}
    public string CreatedBy { get;  set;}
    public DateTime? ModifiedOn { get;  set;}
    public string? ModifiedBy { get;  set;}
    public DateTime? RevokedOn { get;  set;}
    public string? RevokedBy { get;  set;}
    
}
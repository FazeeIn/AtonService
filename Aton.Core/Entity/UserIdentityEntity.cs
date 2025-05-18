namespace Aton.DataBase.Entity;

public class UserIdentityEntity
{
    public Guid Id { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public int? Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public string? AdminLogin { get; set; }
}
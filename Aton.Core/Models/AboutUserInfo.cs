namespace Aton.Core.Models;

public class AboutUserInfo
{
    public string Name { get; }
    public int Gender { get; }
    public DateTime? Birthday { get; }
    public bool IsActive { get; }
    
    public AboutUserInfo(string name, int gender, DateTime? birthday, bool isActive)
    {
        Name = name;
        Gender = gender;
        Birthday = birthday;
        IsActive = isActive;
    }
}
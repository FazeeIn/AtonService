using Aton.DataBase.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aton.DataBase.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        var id = Guid.NewGuid();
        var date = DateTime.UtcNow;

        builder.HasData(new UserEntity
        {
            Id = id,
            Login = "adminlogin",
            Password = "adminpassword",
            Name = "dadada",
            Gender = 0,
            Birthday = date,
            Admin = true,
            CreatedBy = "void",
            CreatedOn = date
        });
    }
}
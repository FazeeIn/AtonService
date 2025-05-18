using Aton.DataBase.Entity;
using Microsoft.EntityFrameworkCore;

namespace Aton.DataBase;

public class DataBaseContext: DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    
    public DataBaseContext(DbContextOptions<DataBaseContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var id = Guid.NewGuid();
        var date = DateTime.UtcNow;
        
        modelBuilder.Entity<UserEntity>().HasData(new UserEntity
        {
            Id = id, 
            Login= "adminlogin",
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
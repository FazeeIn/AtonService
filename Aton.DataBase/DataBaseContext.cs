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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataBaseContext).Assembly);
    }
}
using BeeApps.Common.Enumerations;
using BeeApps.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace BeeApps.Common.Contexts;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var permission in (PermissionName[])Enum.GetValues(typeof(PermissionName)))
            modelBuilder.Entity<Permission>().HasData(new Permission
                { Id = (int)permission, Name = permission.ToString() });
    }
}
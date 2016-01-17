using Microsoft.AspNet.Identity.EntityFramework;
using StockScream.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StockScream.Services
{
    //public class UserDataContext : DbContext
    //{
    //    public DbSet<UserData> Users { get; set; }
    //    public DbSet<Plan> Plans { get; set; }
    //    public UserDataContext() : base("UserDbConnection")
    //    {
    //    }
    //}

    //public class UserDbContext : DbContext
    //{
    //    public DbSet<ApplicationUser> Users { get; set; }
    //    public UserDbContext() : base("DefaultConnection")
    //    {
    //    }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);

    //        modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
    //        modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
    //        modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });
    //    }
    //}
}
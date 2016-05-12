using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace StockScream.Identity
{
    /// <summary>
    /// http://www.codeguru.com/csharp/article.php/c19999/Understanding-Database-Initializers-in-Entity-Framework-Code-First.htm
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //public DbSet<ApplicationUser> Users { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            // set to migrate automatically without losing old data => Migration configuration is used for migrating
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>("DefaultConnection"));
           
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            System.Data.Entity.Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

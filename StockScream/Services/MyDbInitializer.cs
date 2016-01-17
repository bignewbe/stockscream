using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StockScream.DataModels;
using StockScream.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MongoDB.Driver.Linq;
using MongoDB.Driver;


namespace StockScream.Services
{
    public class MyDbInitializer
    {
        /// <summary>
        /// populate the database from saved data. existing data is handled since 'AddOrUpdate' is used
        /// </summary>
        /// <param name="context"></param>
        public static void InitializeDb(ApplicationDbContext context)
        {
            //add users
            var hasher = new PasswordHasher();
            var users = new List<ApplicationUser> {
                new ApplicationUser{PasswordHash = hasher.HashPassword("Abc123!"), Email = "imliping@gmail.com", UserName = "imliping@gmail.com",  EmailConfirmed=true},
            };
            context.Users.AddOrUpdate(u => u.Email, users.ToArray());
            SaveDbContext(context);

            //add roles
            var roleNames = new List<string> { "Admin", "Free", "Basic", "Advanced", "Ultimate" };
            context.Roles.AddOrUpdate(r => r.Name, roleNames.Select(r => new IdentityRole(r)).ToArray());
            SaveDbContext(context);

            //assign roles to each user
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);
            var user = userManager.FindByEmail("imliping@gmail.com");
            var userRoles = userManager.GetRoles(user.Id);
            foreach (var name in roleNames)
            {
                if (!userRoles.Contains(name))
                    userManager.AddToRole(user.Id, name);
            }
            SaveDbContext(context);
        }

        public static void SaveDbContext(ApplicationDbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        /// <summary>
        /// used by ApplicationInitializer to seed empty database after model change. 
        /// 1. is called when DbContext is firsted used
        /// 2. we dont have to handle exising data since table is dropped
        /// 3. we add admin only 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task AddAdmin(ApplicationDbContext context)
        {
            var email = "imliping@gmail.com";
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);
            if (userManager.FindByEmail(email) == null)
            {
                //add all roles if db is empty
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var roleNames = new List<string> { "Admin", "Free", "Basic", "Advanced", "Ultimate" };
                foreach (var roleName in roleNames)
                {
                    if (roleManager.FindByName(roleName) == null)
                    {
                        var role = new IdentityRole { Name = roleName };
                        roleManager.Create(role);
                    }
                }

                //admin
                var user = new ApplicationUser { Email = email, UserName = email, EmailConfirmed = true };
                userManager.Create(user, "Abc123!");
                foreach (var role in roleNames)
                    userManager.AddToRole(user.Id, role);

                //user profile for admin
                var mongoDb = MongoConfig.OpenUsers();
                var profile = await (await mongoDb.FindAsync(u => u.Email == email)).ToListAsync();
                if (profile.Count == 0)
                {
                    var userProfile = new UserProfile { Email = user.Email, FirstName = "Li", LastName = "Ping", Birthday = new Mydate { Year = 1976, Month = 10, Day = 29 } };
                    await mongoDb.InsertOneAsync(userProfile);
                }
            }
            Debug.WriteLine("Seed completed!");
        }

        //reset both sql server express and mongo db
        public static async void ResetDatabases()
        {
            var applicationDb = new ApplicationDbContext();
            var mongoDb = MongoConfig.OpenUsers();

            //remove all users from mongo db and application db
            if (applicationDb.Users.Any())
            {
                await mongoDb.DeleteManyAsync(u => !string.IsNullOrEmpty(u.Email));

                var userStore = new UserStore<ApplicationUser>(applicationDb);
                var userManager = new ApplicationUserManager(userStore);

                var users = userManager.Users.ToList();
                for (int i = 0; i < users.Count; i++)
                    await userManager.DeleteAsync(users[i]);
            }

            //remove all roles
            if (applicationDb.Roles.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(applicationDb);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var roles = roleManager.Roles.ToList();
                for (int i = 0; i < roles.Count; i++)
                    await roleManager.DeleteAsync(roles[i]);
            }

            var roleNames = new List<string> { "Admin", "Basic", "Advanced", "Ultimate" };
            if (!applicationDb.Roles.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(applicationDb);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                foreach (var roleName in roleNames)
                {
                    var role = new IdentityRole { Name = roleName };
                    roleManager.Create(role);
                }
            }

            //add administrator
            if (!applicationDb.Users.Any())
            {
                var userStore = new UserStore<ApplicationUser>(applicationDb);
                var userManager = new ApplicationUserManager(userStore);
                var user = new ApplicationUser
                {
                    Email = "imliping@gmail.com",
                    UserName = "imliping@gmail.com",
                    EmailConfirmed = true
                };
                userManager.Create(user, "Abc123!");

                var userProfile = new UserProfile { Email = user.Email, FirstName = "Li", LastName = "Ping", Birthday = new Mydate { Year = 1976, Month = 10, Day = 29 } };
                foreach (var role in roleNames)
                {
                    userManager.AddToRole(user.Id, role);
                    //userProfile.Roles.Add(role);
                }

                await mongoDb.InsertOneAsync(userProfile);

                //await db.DeleteManyAsync(u => !string.IsNullOrEmpty(u.FirstName));
                //var profile = await (await db.FindAsync(u => u.Email == "imliping@gmail.com")).ToListAsync();
                //var found = await (await profiles.FindAsync(u => u.FirstName == "Ping" && u.LastName == "Li")).ToListAsync();            
                //await profiles.InsertOneAsync(new UserProfile("Ping", "Li"));
            }
            Debug.WriteLine("Seed completed!");
        }

    }
}
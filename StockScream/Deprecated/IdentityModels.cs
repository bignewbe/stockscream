namespace StockScream.Models
{
    //// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public Mydate Birthday { get; set; }
    //    public MyCreditcard CreditCard { get; set; }
    //    public MyAddress BillingAddress { get; set; }
    //    public Plan SubscribPlan { get; set; }
    //    public Token Token { get; set; }
    //    public virtual List<Plan> PlanHistory { get; set; }

    //    //public ICollection<string> Roles { get; set; }
    //    [NotMapped]
    //    public Dictionary<string, string> FiltersFA { get; set; }
    //    [NotMapped]
    //    public Dictionary<string, string> FiltersW { get; set; }
    //    [NotMapped]
    //    public Dictionary<string, string> FiltersM { get; set; }

    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //        // Add custom user claims here
    //        return userIdentity;
    //    }

    //    public ApplicationUser()
    //    {
    //        FirstName = LastName = string.Empty;
    //        Birthday = new Mydate();
    //        CreditCard = new MyCreditcard();
    //        BillingAddress = new MyAddress();
    //        SubscribPlan = new Plan();
    //        PlanHistory = new List<Plan>();
    //        FiltersFA = new Dictionary<string, string>();
    //        FiltersW = new Dictionary<string, string>();
    //        FiltersM = new Dictionary<string, string>();
    //        Token = new Token();
    //    }
    //}

    //// This is useful if you do not want to tear down the database each time you run the application.
    //// public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    //// This example shows you how to create a new database if the Model changes
    //public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    //{
    //    public override async void InitializeDatabase(ApplicationDbContext context)
    //    {
    //        base.InitializeDatabase(context);
    //        await InitializeDatabases(context);
    //    }

    //    async Task InitializeDatabases(ApplicationDbContext context)
    //    {
    //        //var applicationDb = new ApplicationDbContext();
    //        //var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
    //        //var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
    //        var email = "imliping@gmail.com";
    //        var userStore = new UserStore<ApplicationUser>(context);
    //        var userManager = new ApplicationUserManager(userStore);
    //        if (userManager.FindByEmail(email) == null) {
    //            //add all roles if db is empty
    //            var roleStore = new RoleStore<IdentityRole>(context);
    //            var roleManager = new RoleManager<IdentityRole>(roleStore);
    //            var roleNames = new List<string> { "Admin", "Free", "Basic", "Advanced", "Ultimate" };
    //            foreach (var roleName in roleNames) {
    //                if (roleManager.FindByName(roleName) == null) {
    //                    var role = new IdentityRole { Name = roleName };
    //                    roleManager.Create(role);
    //                }
    //            }

    //            //admin
    //            var user = new ApplicationUser { Email = email, UserName = email, EmailConfirmed = true };
    //            userManager.Create(user, "Abc123!");
    //            foreach (var role in roleNames)
    //                userManager.AddToRole(user.Id, role);

    //            //user profile for admin
    //            var mongoDb = MongoConfig.OpenUsers();
    //            var profile = await (await mongoDb.FindAsync(u => u.Email == email)).ToListAsync();
    //            if (profile.Count == 0) {
    //                var userProfile = new UserProfile { Email = user.Email, FirstName = "Li", LastName = "Ping", Birthday = new Mydate { Year = 1976, Month = 10, Day = 29 } };
    //                await mongoDb.InsertOneAsync(userProfile);
    //            }
    //        }
    //        Debug.WriteLine("Seed completed!");
    //    }

    //    //reset both sql server express and mongo db
    //    public static async void ResetDatabases()
    //    {
    //        var applicationDb = new ApplicationDbContext();
    //        var mongoDb = MongoConfig.OpenUsers();

    //        //remove all users from mongo db and application db
    //        if (applicationDb.Users.Any()) {
    //            await mongoDb.DeleteManyAsync(u => !string.IsNullOrEmpty(u.Email));

    //            var userStore = new UserStore<ApplicationUser>(applicationDb);
    //            var userManager = new ApplicationUserManager(userStore);

    //            var users = userManager.Users.ToList();
    //            for (int i = 0; i < users.Count; i++)
    //                await userManager.DeleteAsync(users[i]);
    //        }

    //        //remove all roles
    //        if (applicationDb.Roles.Any()) {
    //            var roleStore = new RoleStore<IdentityRole>(applicationDb);
    //            var roleManager = new RoleManager<IdentityRole>(roleStore);
    //            var roles = roleManager.Roles.ToList();
    //            for (int i = 0; i < roles.Count; i++)
    //                await roleManager.DeleteAsync(roles[i]);
    //        }

    //        var roleNames = new List<string> { "Admin", "Basic", "Advanced", "Ultimate" };
    //        if (!applicationDb.Roles.Any()) {
    //            var roleStore = new RoleStore<IdentityRole>(applicationDb);
    //            var roleManager = new RoleManager<IdentityRole>(roleStore);
    //            foreach (var roleName in roleNames) {
    //                var role = new IdentityRole { Name = roleName };
    //                roleManager.Create(role);
    //            }
    //        }

    //        //add administrator
    //        if (!applicationDb.Users.Any()) {
    //            var userStore = new UserStore<ApplicationUser>(applicationDb);
    //            var userManager = new ApplicationUserManager(userStore);
    //            var user = new ApplicationUser
    //            {
    //                Email = "imliping@gmail.com",
    //                UserName = "imliping@gmail.com",
    //                EmailConfirmed = true
    //            };
    //            userManager.Create(user, "Abc123!");

    //            var userProfile = new UserProfile { Email = user.Email, FirstName = "Li", LastName = "Ping", Birthday = new Mydate { Year = 1976, Month = 10, Day = 29 } };
    //            foreach (var role in roleNames) {
    //                userManager.AddToRole(user.Id, role);
    //                //userProfile.Roles.Add(role);
    //            }

    //            await mongoDb.InsertOneAsync(userProfile);

    //            //await db.DeleteManyAsync(u => !string.IsNullOrEmpty(u.FirstName));
    //            //var profile = await (await db.FindAsync(u => u.Email == "imliping@gmail.com")).ToListAsync();
    //            //var found = await (await profiles.FindAsync(u => u.FirstName == "Ping" && u.LastName == "Li")).ToListAsync();            
    //            //await profiles.InsertOneAsync(new UserProfile("Ping", "Li"));
    //        }
    //        Debug.WriteLine("Seed completed!");
    //    }

    //    ////this function usually is triggered by first time log-in or register. 
    //    ////and only when model (ApplicationUser) is changed. 
    //    //protected override void Seed(ApplicationDbContext context)
    //    //{
    //    //    base.Seed(context);
    //    //    InitializeDatabases();
    //    //}

    //    //public static void InitializeIdentityForEF(ApplicationDbContext db)
    //    //{
    //    //    var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
    //    //    var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
    //    //    const string name = "admin@example.com";
    //    //    const string password = "Abc123!";
    //    //    const string roleName = "Admin";

    //    //    //Create Role Admin if it does not exist
    //    //    var role = roleManager.FindByName(roleName);
    //    //    if (role == null)
    //    //    {
    //    //        role = new IdentityRole(roleName);
    //    //        var roleresult = roleManager.Create(role);
    //    //    }

    //    //    var user = userManager.FindByName(name);
    //    //    if (user == null)
    //    //    {
    //    //        user = new ApplicationUser { UserName = name, Email = name };
    //    //        var result = userManager.Create(user, password);
    //    //        result = userManager.SetLockoutEnabled(user.Id, false);
    //    //    }

    //    //    // Add user admin to Role Admin if not already added
    //    //    var rolesForUser = userManager.GetRoles(user.Id);
    //    //    if (!rolesForUser.Contains(role.Name))
    //    //    {
    //    //        var result = userManager.AddToRole(user.Id, role.Name);
    //    //    }
    //    //}
    //}

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    //public DbSet<ApplicationUser> Users { get; set; }
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //        //set to migrate automatically without losing old data => Migration configuration is used for migrating
    //        //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Migrations.Configuration>("DefaultConnection"));
    //        // Set the database intializer which is run once during application start
    //        // This seeds the database with admin user credentials and admin role
    //        Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}
}
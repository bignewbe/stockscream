using System.Data.Entity;
using StockScream.Services;

namespace StockScream.Identity
{
    //  Database initializers run the first time that a DbContext is used and can do things like check if a database 
    //  already exists and create a new database if needed.
    //  1. Checks whether or not the target database already exists
    //  2. If it does, then the current Code First model is compared with the model stored in metadata in the database
    //  3. The database is dropped if the current model does not match the model in the database
    //  4. The database is created if it was dropped or didn’t exist in the first place
    //  5. If the database was created, then the initializer Seed method is called
    //    a. Database initializer Seed methods do not have to handle existing data. That is, new entities can be 
    //       inserted without any need to check whether or not the entities already exist in the database.
    //    b. The Seed method will not be called when the application is run if the database already exists 
    //       and the model has not changed since the last run. We’ll come back to this point later.
    // please note that once a database is dropped, all old data is lost. we need to use migration in order to avoid 
    // losing old data. 
    //public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    //public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        public override void InitializeDatabase(ApplicationDbContext context)
        {
            base.InitializeDatabase(context);

            MyDbInitializer.AddAdmin(context);
        }
    }
}

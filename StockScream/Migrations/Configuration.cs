namespace StockScream.Migrations
{
    using Identity;
    using Services;
    using System.Data.Entity.Migrations;
    // EF 4.3 introduced Code First Migrations.Migrations provide a way for the database to be evolved without needing to 
    // drop and recreate the entire database. Use of Migrations commonly involves using PowerShell commands to manage updates
    // to the database explicitly.That is, database creation and updates are usually handled during development from PowerShell
    // and do not happen automatically when the applications runs. (See The Migrations initializer below for how this can be changed.)
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        // When you are dropping and re-creating the database for every data model change, you use the initializer class's Seed method to insert test data, 
        // because after every model change the database is dropped and old data is lost. 
        // With Code First Migrations, test data is retained after
        // database changes, so including test data in the Seed method is typically not necessary. In fact, you don't want the Seed method to insert test
        // data if you'll be using Migrations to deploy the database to production, because the Seed method will run in production. In that case you want 
        // the Seed method to insert into the database only the data that you need in production. For example, you might want the database to include actual
        // department names in the Department table when the application becomes available in production.
        //1. seed is run every time Update-Database is run in terminal. it will never be run when application start
        //2. AddOrUpdate take care of 'upinsert'
        protected override void Seed(ApplicationDbContext context)
        {
            MyDbInitializer.InitializeDb(context);                        
        }
    }
}

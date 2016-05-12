namespace StockScream.Services
{
    public static class MongoConfig
    {
        //static IMongoCollection<dynamic> OpenDataBase(string connectionStr, string dbName)
        //{
        //    //var client = new MongoClient("mongodb://localhost");
        //    //string connectionStr = ConfigurationManager.ConnectionStrings["MongoDbConnection"].ConnectionString;
        //    //var dbName = "Users";
        //    var client = new MongoClient(connectionStr);
        //    var db = client.GetDatabase(dbName);
        //    return db.GetCollection<dynamic>(dbName);
        //}

        //public static IMongoCollection<UserProfile> OpenUsers()
        //{
        //    //var client = new MongoClient("mongodb://localhost");
        //    string connectionStr = ConfigurationManager.ConnectionStrings["MongoDbConnection"].ConnectionString;
        //    var client = new MongoClient(connectionStr);
        //    var db = client.GetDatabase("Users");
        //    return db.GetCollection<UserProfile>("Users");
        //}

        //public static IMongoCollection<StockMapping> OpenStockMapping()
        //{
        //    //var client = new MongoClient("mongodb://localhost");
        //    string connectionStr = ConfigurationManager.ConnectionStrings["MongoDbConnection"].ConnectionString;
        //    var client = new MongoClient(connectionStr);
        //    var db = client.GetDatabase("StockMapping");
        //    return db.GetCollection<StockMapping>("StockMapping");
        //}

        //public static async void Seed()
        //{
        //    var profiles = OpenUsers();
        //    var found = await (await profiles.FindAsync(u => u.FirstName == "Ping" && u.LastName == "Li")).ToListAsync();
        //    await profiles.DeleteManyAsync(u => !string.IsNullOrEmpty(u.FirstName));

        //    //var found = await (await profiles.FindAsync(u => u.FirstName == "Ping" && u.LastName == "Li")).ToListAsync();            
        //    //await profiles.InsertOneAsync(new UserProfile("Ping", "Li"));

        //    ////update FirstName 
        //    //await profiles.UpdateOneAsync(u => u.FirstName == "Ping" && u.LastName == "Li", 
        //    //    Builders<UserProfile>.Update.Set(u=>u.FirstName, "Peter"));

        //    //found = await (await profiles.FindAsync(u => u.FirstName == "Peter")).ToListAsync();
        //    //if (found == null || found.Count == 0)
        //    //{
        //    //    var data = new List<UserProfile>
        //    //    {
        //    //        new UserProfile("Ping", "Li"),
        //    //        new UserProfile("Ying", "Li")
        //    //    };
        //    //    found = await (await profiles.FindAsync(u => u.FirstName == "Ping" && u.LastName == "Li")).ToListAsync();
        //    //}
        //}
    }
}
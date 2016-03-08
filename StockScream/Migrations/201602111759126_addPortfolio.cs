namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPortfolio : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "JsonPortofolio", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "JsonPortofolio");
        }
    }
}

namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTokenAgain : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserProfiles", "Token_AuthToken");
            DropColumn("dbo.UserProfiles", "Token_IssuedOn");
            DropColumn("dbo.UserProfiles", "Token_ExpiresOn");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfiles", "Token_ExpiresOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserProfiles", "Token_IssuedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserProfiles", "Token_AuthToken", c => c.String());
        }
    }
}

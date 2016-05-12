namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfiles", "Token_AuthToken", c => c.String());
            AddColumn("dbo.UserProfiles", "Token_IssuedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserProfiles", "Token_ExpiresOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfiles", "Token_ExpiresOn");
            DropColumn("dbo.UserProfiles", "Token_IssuedOn");
            DropColumn("dbo.UserProfiles", "Token_AuthToken");
        }
    }
}

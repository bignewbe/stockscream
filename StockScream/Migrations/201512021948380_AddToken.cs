namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddToken : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Token_AuthToken", c => c.String());
            AddColumn("dbo.AspNetUsers", "Token_IssuedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "Token_ExpiresOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Token_ExpiresOn");
            DropColumn("dbo.AspNetUsers", "Token_IssuedOn");
            DropColumn("dbo.AspNetUsers", "Token_AuthToken");
        }
    }
}

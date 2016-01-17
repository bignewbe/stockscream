namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserIdFromPlan : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Plans", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Plans", "UserId", c => c.Int(nullable: false));
        }
    }
}

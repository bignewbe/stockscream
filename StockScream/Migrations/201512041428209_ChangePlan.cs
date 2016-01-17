namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePlan : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "SubscribPlan_StartDate", "dbo.Plans");
            DropIndex("dbo.AspNetUsers", new[] { "SubscribPlan_StartDate" });
            DropPrimaryKey("dbo.Plans");
            AddColumn("dbo.Plans", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Plans", "UserId", c => c.Int(nullable: false));
            AlterColumn("dbo.Plans", "StartDate", c => c.String());
            AddPrimaryKey("dbo.Plans", "Id");
            DropColumn("dbo.AspNetUsers", "SubscribPlan_StartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "SubscribPlan_StartDate", c => c.String(maxLength: 128));
            DropPrimaryKey("dbo.Plans");
            AlterColumn("dbo.Plans", "StartDate", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Plans", "UserId");
            DropColumn("dbo.Plans", "Id");
            AddPrimaryKey("dbo.Plans", "StartDate");
            CreateIndex("dbo.AspNetUsers", "SubscribPlan_StartDate");
            AddForeignKey("dbo.AspNetUsers", "SubscribPlan_StartDate", "dbo.Plans", "StartDate");
        }
    }
}

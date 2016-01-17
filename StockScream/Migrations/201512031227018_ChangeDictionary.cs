namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDictionary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "XmlFilterFA", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "XmlFilterFA");
        }
    }
}

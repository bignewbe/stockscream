namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDictionary : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "XmlFilterFA", c => c.String());
            AddColumn("dbo.AspNetUsers", "XmlFiltersW", c => c.String());
            AddColumn("dbo.AspNetUsers", "XmlFiltersM", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "XmlFiltersM");
            DropColumn("dbo.AspNetUsers", "XmlFiltersW");
            DropColumn("dbo.AspNetUsers", "XmlFilterFA");
        }
    }
}

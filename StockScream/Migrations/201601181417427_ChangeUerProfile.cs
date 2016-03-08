namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeUerProfile : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Plans", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Plans", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AspNetUsers", "FirstName");
            DropColumn("dbo.AspNetUsers", "LastName");
            DropColumn("dbo.AspNetUsers", "Birthday_Year");
            DropColumn("dbo.AspNetUsers", "Birthday_Month");
            DropColumn("dbo.AspNetUsers", "Birthday_Day");
            DropColumn("dbo.AspNetUsers", "CreditCard_IsValid");
            DropColumn("dbo.AspNetUsers", "CreditCard_NameOnCard");
            DropColumn("dbo.AspNetUsers", "CreditCard_Type");
            DropColumn("dbo.AspNetUsers", "CreditCard_Number");
            DropColumn("dbo.AspNetUsers", "CreditCard_ExpMonth");
            DropColumn("dbo.AspNetUsers", "CreditCard_ExpYear");
            DropColumn("dbo.AspNetUsers", "BillingAddress_Country");
            DropColumn("dbo.AspNetUsers", "BillingAddress_State");
            DropColumn("dbo.AspNetUsers", "BillingAddress_City");
            DropColumn("dbo.AspNetUsers", "BillingAddress_Street");
            DropColumn("dbo.AspNetUsers", "BillingAddress_HouseNumber");
            DropColumn("dbo.AspNetUsers", "BillingAddress_Postcode");
            DropColumn("dbo.AspNetUsers", "Token_AuthToken");
            DropColumn("dbo.AspNetUsers", "Token_IssuedOn");
            DropColumn("dbo.AspNetUsers", "Token_ExpiresOn");
            DropColumn("dbo.AspNetUsers", "XmlFilterFA");
            DropColumn("dbo.AspNetUsers", "XmlFiltersW");
            DropColumn("dbo.AspNetUsers", "XmlFiltersM");
            DropColumn("dbo.Plans", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Plans", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "XmlFiltersM", c => c.String());
            AddColumn("dbo.AspNetUsers", "XmlFiltersW", c => c.String());
            AddColumn("dbo.AspNetUsers", "XmlFilterFA", c => c.String());
            AddColumn("dbo.AspNetUsers", "Token_ExpiresOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "Token_IssuedOn", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "Token_AuthToken", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_Postcode", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_HouseNumber", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_Street", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_City", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_State", c => c.String());
            AddColumn("dbo.AspNetUsers", "BillingAddress_Country", c => c.String());
            AddColumn("dbo.AspNetUsers", "CreditCard_ExpYear", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "CreditCard_ExpMonth", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "CreditCard_Number", c => c.String());
            AddColumn("dbo.AspNetUsers", "CreditCard_Type", c => c.String());
            AddColumn("dbo.AspNetUsers", "CreditCard_NameOnCard", c => c.String());
            AddColumn("dbo.AspNetUsers", "CreditCard_IsValid", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Birthday_Day", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Birthday_Month", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Birthday_Year", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "LastName", c => c.String());
            AddColumn("dbo.AspNetUsers", "FirstName", c => c.String());
            CreateIndex("dbo.Plans", "ApplicationUser_Id");
            AddForeignKey("dbo.Plans", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}

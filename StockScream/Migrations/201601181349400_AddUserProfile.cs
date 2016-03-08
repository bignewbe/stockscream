namespace StockScream.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProfile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Birthday_Year = c.Int(nullable: false),
                        Birthday_Month = c.Int(nullable: false),
                        Birthday_Day = c.Int(nullable: false),
                        CreditCard_IsValid = c.Boolean(nullable: false),
                        CreditCard_NameOnCard = c.String(),
                        CreditCard_Type = c.String(),
                        CreditCard_Number = c.String(),
                        CreditCard_ExpMonth = c.Int(nullable: false),
                        CreditCard_ExpYear = c.Int(nullable: false),
                        BillingAddress_Country = c.String(),
                        BillingAddress_State = c.String(),
                        BillingAddress_City = c.String(),
                        BillingAddress_Street = c.String(),
                        BillingAddress_HouseNumber = c.String(),
                        BillingAddress_Postcode = c.String(),
                        Token_AuthToken = c.String(),
                        Token_IssuedOn = c.DateTime(nullable: false),
                        Token_ExpiresOn = c.DateTime(nullable: false),
                        XmlFilterFA = c.String(),
                        XmlFiltersW = c.String(),
                        XmlFiltersM = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "Profile_Id", c => c.Int());
            AddColumn("dbo.Plans", "UserProfile_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "Profile_Id");
            CreateIndex("dbo.Plans", "UserProfile_Id");
            AddForeignKey("dbo.Plans", "UserProfile_Id", "dbo.UserProfiles", "Id");
            AddForeignKey("dbo.AspNetUsers", "Profile_Id", "dbo.UserProfiles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Profile_Id", "dbo.UserProfiles");
            DropForeignKey("dbo.Plans", "UserProfile_Id", "dbo.UserProfiles");
            DropIndex("dbo.Plans", new[] { "UserProfile_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "Profile_Id" });
            DropColumn("dbo.Plans", "UserProfile_Id");
            DropColumn("dbo.AspNetUsers", "Profile_Id");
            DropTable("dbo.UserProfiles");
        }
    }
}

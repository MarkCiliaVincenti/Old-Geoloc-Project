namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIndetifierEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Identifiers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DriverId = c.Guid(),
                        CardNumber = c.Long(nullable: false),
                        CardIssueDate = c.DateTime(),
                        CardValidityBegin = c.DateTime(),
                        CardExpiryDate = c.DateTime(),
                        CustomerId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Drivers", t => t.DriverId)
                .Index(t => t.DriverId)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Identifiers", "DriverId", "dbo.Drivers");
            DropForeignKey("dbo.Identifiers", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Identifiers", new[] { "CustomerId" });
            DropIndex("dbo.Identifiers", new[] { "DriverId" });
            DropTable("dbo.Identifiers");
        }
    }
}

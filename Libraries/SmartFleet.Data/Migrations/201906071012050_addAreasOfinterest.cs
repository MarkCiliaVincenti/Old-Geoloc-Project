namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAreasOfinterest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InterestAreas",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        CustomerId = c.Guid(nullable: false),
                        Country = c.String(),
                        State = c.String(),
                        Street = c.String(),
                        Logitude = c.Single(nullable: false),
                        Latitude = c.Single(nullable: false),
                        Radius = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateIndex("dbo.Positions", "Box_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InterestAreas", "CustomerId", "dbo.Customers");
            DropIndex("dbo.InterestAreas", new[] { "CustomerId" });
            DropIndex("dbo.Positions", new[] { "Box_Id" });
            DropTable("dbo.InterestAreas");
        }
    }
}

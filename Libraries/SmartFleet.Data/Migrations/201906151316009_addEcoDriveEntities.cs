namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEcoDriveEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FuelConsumptions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VehicleId = c.Guid(nullable: false),
                        CustomerId = c.Guid(nullable: false),
                        FuelLevel = c.Int(nullable: false),
                        FuelUsed = c.Int(nullable: false),
                        Milestone = c.Long(nullable: false),
                        DateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VehicleAlerts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CustomerId = c.Guid(nullable: false),
                        VehicleId = c.Guid(nullable: false),
                        EventUtc = c.DateTime(nullable: false),
                        VehicleEvent = c.Int(nullable: false),
                        Speed = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Vehicles", t => t.VehicleId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.VehicleId);
            
            AddColumn("dbo.Vehicles", "SpeedAlertEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Vehicles", "CANEnabled", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Vehicles", "MaxSpeed", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VehicleAlerts", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.VehicleAlerts", "CustomerId", "dbo.Customers");
            DropIndex("dbo.VehicleAlerts", new[] { "VehicleId" });
            DropIndex("dbo.VehicleAlerts", new[] { "CustomerId" });
            AlterColumn("dbo.Vehicles", "MaxSpeed", c => c.Int(nullable: false));
            DropColumn("dbo.Vehicles", "CANEnabled");
            DropColumn("dbo.Vehicles", "SpeedAlertEnabled");
            DropTable("dbo.VehicleAlerts");
            DropTable("dbo.FuelConsumptions");
        }
    }
}

namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addvehicleAndCusstomerFuel : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.FuelConsumptions", "VehicleId");
            CreateIndex("dbo.FuelConsumptions", "CustomerId");
            AddForeignKey("dbo.FuelConsumptions", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FuelConsumptions", "VehicleId", "dbo.Vehicles", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FuelConsumptions", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.FuelConsumptions", "CustomerId", "dbo.Customers");
            DropIndex("dbo.FuelConsumptions", new[] { "CustomerId" });
            DropIndex("dbo.FuelConsumptions", new[] { "VehicleId" });
        }
    }
}

namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtotalFuel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FuelConsumptions", "TotalFuelConsumed", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FuelConsumptions", "TotalFuelConsumed");
        }
    }
}

namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vehicleOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicles", "EcoDrive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Vehicles", "TachoEnabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vehicles", "TachoEnabled");
            DropColumn("dbo.Vehicles", "EcoDrive");
        }
    }
}

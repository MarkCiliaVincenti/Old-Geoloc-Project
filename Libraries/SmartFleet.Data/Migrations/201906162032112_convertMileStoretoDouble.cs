namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class convertMileStoretoDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FuelConsumptions", "Milestone", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.FuelConsumptions", "Milestone", c => c.Long(nullable: false));
        }
    }
}

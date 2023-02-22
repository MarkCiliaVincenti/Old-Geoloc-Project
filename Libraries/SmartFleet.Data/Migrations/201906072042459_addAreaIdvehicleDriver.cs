namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAreaIdvehicleDriver : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicles", "InterestAreaId", c => c.Guid());
            AddColumn("dbo.Vehicles", "InterestArea_Id", c => c.Guid());
            AddColumn("dbo.Drivers", "InterestAreaId", c => c.Guid());
            AddColumn("dbo.Drivers", "InterestArea_Id", c => c.Guid());
            CreateIndex("dbo.Vehicles", "InterestArea_Id");
            CreateIndex("dbo.Drivers", "InterestArea_Id");
            AddForeignKey("dbo.Drivers", "InterestArea_Id", "dbo.InterestAreas", "Id");
            AddForeignKey("dbo.Vehicles", "InterestArea_Id", "dbo.InterestAreas", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Vehicles", "InterestArea_Id", "dbo.InterestAreas");
            DropForeignKey("dbo.Drivers", "InterestArea_Id", "dbo.InterestAreas");
            DropIndex("dbo.Drivers", new[] { "InterestArea_Id" });
            DropIndex("dbo.Vehicles", new[] { "InterestArea_Id" });
            DropColumn("dbo.Drivers", "InterestArea_Id");
            DropColumn("dbo.Drivers", "InterestAreaId");
            DropColumn("dbo.Vehicles", "InterestArea_Id");
            DropColumn("dbo.Vehicles", "InterestAreaId");
        }
    }
}

namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeMilestoneTypetoDouble : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vehicles", "MileStoneUpdateUtc", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Vehicles", "Milestone", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Vehicles", "Milestone", c => c.Int(nullable: false));
            DropColumn("dbo.Vehicles", "MileStoneUpdateUtc");
        }
    }
}

namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixxMissSpeling : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InterestAreas", "Longitude", c => c.Single(nullable: false));
            DropColumn("dbo.InterestAreas", "Logitude");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InterestAreas", "Logitude", c => c.Single(nullable: false));
            DropColumn("dbo.InterestAreas", "Longitude");
        }
    }
}

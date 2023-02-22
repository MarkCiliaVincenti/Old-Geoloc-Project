namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changespeedtype : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Positions", "Speed", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Positions", "Speed", c => c.Short(nullable: false));
        }
    }
}

namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLastGpsinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boxes", "LastGpsInfoTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boxes", "LastGpsInfoTime");
        }
    }
}

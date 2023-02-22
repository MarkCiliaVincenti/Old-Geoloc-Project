namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trunDatesTiNullBox : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Boxes", "CreationDate", c => c.DateTime());
            AlterColumn("dbo.Boxes", "LastGpsInfoTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Boxes", "LastGpsInfoTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Boxes", "CreationDate", c => c.DateTime(nullable: false));
        }
    }
}

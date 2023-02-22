namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBrandTypeToBox : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boxes", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Boxes", "Brand", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boxes", "Brand");
            DropColumn("dbo.Boxes", "Type");
        }
    }
}

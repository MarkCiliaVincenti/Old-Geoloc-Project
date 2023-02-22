namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAddressToPositionEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Positions", "Address", c => c.String());
            AddColumn("dbo.Positions", "Region", c => c.String());
            AddColumn("dbo.Positions", "State", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Positions", "State");
            DropColumn("dbo.Positions", "Region");
            DropColumn("dbo.Positions", "Address");
        }
    }
}

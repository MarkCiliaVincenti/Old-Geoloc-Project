namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeTonullable : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Vehicles", name: "InterestArea_Id", newName: "InterestAreaId");
            RenameIndex(table: "dbo.Vehicles", name: "IX_InterestArea_Id", newName: "IX_InterestAreaId");
            AlterColumn("dbo.Vehicles", "MileStoneUpdateUtc", c => c.DateTime());
            DropColumn("dbo.Vehicles", "InteerestAreaId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Vehicles", "InteerestAreaId", c => c.Guid());
            AlterColumn("dbo.Vehicles", "MileStoneUpdateUtc", c => c.DateTime(nullable: false));
            RenameIndex(table: "dbo.Vehicles", name: "IX_InterestAreaId", newName: "IX_InterestArea_Id");
            RenameColumn(table: "dbo.Vehicles", name: "InterestAreaId", newName: "InterestArea_Id");
        }
    }
}

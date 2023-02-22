namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMotionStatus : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Vehicles", "Brand_Id");
            DropColumn("dbo.Vehicles", "Model_Id");
            RenameColumn(table: "dbo.Vehicles", name: "Brand_Id1", newName: "Brand_Id");
            RenameColumn(table: "dbo.Vehicles", name: "Model_Id1", newName: "Model_Id");
            RenameIndex(table: "dbo.Vehicles", name: "IX_Brand_Id1", newName: "IX_Brand_Id");
            RenameIndex(table: "dbo.Vehicles", name: "IX_Model_Id1", newName: "IX_Model_Id");
            AddColumn("dbo.Positions", "MotionStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Positions", "MotionStatus");
            RenameIndex(table: "dbo.Vehicles", name: "IX_Model_Id", newName: "IX_Model_Id1");
            RenameIndex(table: "dbo.Vehicles", name: "IX_Brand_Id", newName: "IX_Brand_Id1");
            RenameColumn(table: "dbo.Vehicles", name: "Model_Id", newName: "Model_Id1");
            RenameColumn(table: "dbo.Vehicles", name: "Brand_Id", newName: "Brand_Id1");
            AddColumn("dbo.Vehicles", "Model_Id", c => c.Guid());
            AddColumn("dbo.Vehicles", "Brand_Id", c => c.Guid());
        }
    }
}

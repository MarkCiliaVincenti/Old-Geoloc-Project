namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class trunDatesToNullCustomer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "CreationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "CreationDate", c => c.DateTime(nullable: false));
        }
    }
}

namespace SmartFleet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Boxes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SerialNumber = c.String(),
                        Imei = c.String(),
                        VehicleId = c.Guid(),
                        BoxStatus = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Icci = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vehicles", t => t.VehicleId)
                .Index(t => t.VehicleId);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Altitude = c.Short(nullable: false),
                        Direction = c.Short(nullable: false),
                        Lat = c.Double(nullable: false),
                        Long = c.Double(nullable: false),
                        Priority = c.Byte(nullable: false),
                        Satellite = c.Byte(nullable: false),
                        Speed = c.Short(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                        Box_Id = c.Guid(),
                        Status = c.String(),
                        StopFlag = c.Boolean(nullable: false),
                        IsStop = c.Boolean(nullable: false),
                        Acceleration = c.Int(nullable: false),
                        DoorStatus = c.Int(nullable: false),
                        Mileage = c.Int(nullable: false),
                        GreedyDriving = c.Int(nullable: false),
                        Box_Id1 = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Boxes", t => t.Box_Id1)
                .Index(t => t.Box_Id1);
            
            CreateTable(
                "dbo.Vehicles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Brand_Id = c.Guid(),
                        Model_Id = c.Guid(),
                        VehicleName = c.String(),
                        Vin = c.String(),
                        LicensePlate = c.String(),
                        CreationDate = c.DateTime(),
                        InitServiceDate = c.DateTime(),
                        VehicleType = c.Int(nullable: false),
                        VehicleStatus = c.Int(nullable: false),
                        CustomerId = c.Guid(),
                        MaxSpeed = c.Int(nullable: false),
                        Milestone = c.Int(nullable: false),
                        Model_Id1 = c.Guid(),
                        Brand_Id1 = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Models", t => t.Model_Id1)
                .ForeignKey("dbo.Brands", t => t.Brand_Id1)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId)
                .Index(t => t.Model_Id1)
                .Index(t => t.Brand_Id1);
            
            CreateTable(
                "dbo.VehicleAlarrms",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VehicleId = c.Guid(),
                        AlarmTime = c.DateTime(nullable: false),
                        AlarmType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vehicles", t => t.VehicleId)
                .Index(t => t.VehicleId);
            
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Models",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Brand_Id = c.Guid(),
                        Brand_Id1 = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.Brand_Id1)
                .Index(t => t.Brand_Id1);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        Tel = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        Country = c.String(),
                        State = c.String(),
                        Street = c.String(),
                        ZipCode = c.String(),
                        CustomerStatus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Tel = c.String(),
                        CustomerId = c.Guid(),
                        TimeZoneInfo = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Tel = c.String(),
                        Email = c.String(),
                        OnService = c.DateTime(),
                        DriverNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.DriverVehicles",
                c => new
                    {
                        Driver_Id = c.Guid(nullable: false),
                        Vehicle_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Driver_Id, t.Vehicle_Id })
                .ForeignKey("dbo.Drivers", t => t.Driver_Id, cascadeDelete: true)
                .ForeignKey("dbo.Vehicles", t => t.Vehicle_Id, cascadeDelete: true)
                .Index(t => t.Driver_Id)
                .Index(t => t.Vehicle_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Boxes", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.DriverVehicles", "Vehicle_Id", "dbo.Vehicles");
            DropForeignKey("dbo.DriverVehicles", "Driver_Id", "dbo.Drivers");
            DropForeignKey("dbo.Vehicles", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.AspNetUsers", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Vehicles", "Brand_Id1", "dbo.Brands");
            DropForeignKey("dbo.Vehicles", "Model_Id1", "dbo.Models");
            DropForeignKey("dbo.Models", "Brand_Id1", "dbo.Brands");
            DropForeignKey("dbo.VehicleAlarrms", "VehicleId", "dbo.Vehicles");
            DropForeignKey("dbo.Positions", "Box_Id1", "dbo.Boxes");
            DropIndex("dbo.DriverVehicles", new[] { "Vehicle_Id" });
            DropIndex("dbo.DriverVehicles", new[] { "Driver_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "CustomerId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Models", new[] { "Brand_Id1" });
            DropIndex("dbo.VehicleAlarrms", new[] { "VehicleId" });
            DropIndex("dbo.Vehicles", new[] { "Brand_Id1" });
            DropIndex("dbo.Vehicles", new[] { "Model_Id1" });
            DropIndex("dbo.Vehicles", new[] { "CustomerId" });
            DropIndex("dbo.Positions", new[] { "Box_Id1" });
            DropIndex("dbo.Boxes", new[] { "VehicleId" });
            DropTable("dbo.DriverVehicles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Drivers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Customers");
            DropTable("dbo.Models");
            DropTable("dbo.Brands");
            DropTable("dbo.VehicleAlarrms");
            DropTable("dbo.Vehicles");
            DropTable("dbo.Positions");
            DropTable("dbo.Boxes");
        }
    }
}

namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeMigration : DbMigration
    {
        public override void Up()
        {
           /* CreateTable(
                "dbo.CancelledDayModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PickedModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        TableId = c.Int(nullable: false),
                        TimeId = c.Int(nullable: false),
                        Length = c.Int(nullable: false),
                        PickedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TableModels", t => t.TableId, cascadeDelete: true)
                .ForeignKey("dbo.TimeModels", t => t.TimeId, cascadeDelete: true)
                .Index(t => t.TableId)
                .Index(t => t.TimeId);
            
            CreateTable(
                "dbo.TableModels",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TimeModels",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ReservationModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        TableId = c.Int(nullable: false),
                        TimeId = c.Int(nullable: false),
                        Length = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TableModels", t => t.TableId, cascadeDelete: true)
                .ForeignKey("dbo.TimeModels", t => t.TimeId, cascadeDelete: true)
                .Index(t => t.TableId)
                .Index(t => t.TimeId);
            
            CreateTable(
                "dbo.SettingModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);*/
            
        }
        
        public override void Down()
        {
           /* DropForeignKey("dbo.ReservationModels", "TimeId", "dbo.TimeModels");
            DropForeignKey("dbo.ReservationModels", "TableId", "dbo.TableModels");
            DropForeignKey("dbo.PickedModels", "TimeId", "dbo.TimeModels");
            DropForeignKey("dbo.PickedModels", "TableId", "dbo.TableModels");
            DropIndex("dbo.ReservationModels", new[] { "TimeId" });
            DropIndex("dbo.ReservationModels", new[] { "TableId" });
            DropIndex("dbo.PickedModels", new[] { "TimeId" });
            DropIndex("dbo.PickedModels", new[] { "TableId" });*/
            DropTable("dbo.SettingModels");
            //DropTable("dbo.ReservationModels");
            //DropTable("dbo.TimeModels");
            //DropTable("dbo.TableModels");
            //DropTable("dbo.PickedModels");
            //DropTable("dbo.CancelledDayModels");
        }
    }
}

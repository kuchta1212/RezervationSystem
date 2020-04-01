namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWeekDay : DbMigration
    {
        public override void Up()
        {/*
            CreateTable(
                "dbo.WeekDayModels",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    StartTime = c.Int(nullable: false),
                    EndTime = c.Int(nullable: false),
                    IsCancelled = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TimeModels", t => t.EndTime, cascadeDelete: false)
                .ForeignKey("dbo.TimeModels", t => t.StartTime, cascadeDelete: false)
                .Index(t => t.StartTime);*/

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WeekDayModels", "StartTime", "dbo.TimeModels");
            DropForeignKey("dbo.WeekDayModels", "EndTime", "dbo.TimeModels");
            DropIndex("dbo.WeekDayModels", new[] { "EndTime" });
            DropIndex("dbo.WeekDayModels", new[] { "StartTime" });
            DropTable("dbo.WeekDayModels");
        }
    }
}

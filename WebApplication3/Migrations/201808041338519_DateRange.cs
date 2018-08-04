namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateRange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DateRangeModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.WeekDayModels", "DateRange", c => c.Int(nullable: false));
            CreateIndex("dbo.WeekDayModels", "DateRange");
            AddForeignKey("dbo.WeekDayModels", "DateRange", "dbo.DateRangeModels", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WeekDayModels", "DateRange", "dbo.DateRangeModels");
            DropIndex("dbo.WeekDayModels", new[] { "DateRange" });
            DropColumn("dbo.WeekDayModels", "DateRange");
            DropTable("dbo.DateRangeModels");
        }
    }
}

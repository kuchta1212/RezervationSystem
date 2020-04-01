namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CancelledDay : DbMigration
    {
        public override void Up()
        {
            /*
            CreateTable(
                "dbo.CancelledDayModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Reason = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            */
        }
        
        public override void Down()
        {
            DropTable("dbo.CancelledDayModels");
        }
    }
}

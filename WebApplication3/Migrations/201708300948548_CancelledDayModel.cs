namespace ReservationSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CancelledDayModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CancelledDayModel",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Reason = c.String(),
                    Date = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Date);
        }
        
        public override void Down()
        {
            DropIndex("dbo.CancelledDayModelCancelledDayModel", new[] { "Date" });
            DropTable("dbo.PickedModels");
        }
    }
}

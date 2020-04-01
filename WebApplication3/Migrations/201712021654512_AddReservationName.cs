namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReservationName : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.ReservationModels", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationModels", "Name");
        }
    }
}

namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTimeStampToPickedModel : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.PickedModels", "TimeStamp", c => c.DateTime(nullable: false));
            //DropColumn("dbo.PickedModels", "Length");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PickedModels", "Length", c => c.Int(nullable: false));
            DropColumn("dbo.PickedModels", "TimeStamp");
        }
    }
}

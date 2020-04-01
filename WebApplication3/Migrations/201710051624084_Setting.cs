namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Setting : DbMigration
    {
        public override void Up()
        {/*
            CreateTable(
                "dbo.SettingModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            */
        }
        
        public override void Down()
        {
            DropTable("dbo.SettingModels");
        }
    }
}

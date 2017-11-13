namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PickedModel : DbMigration
    {
        public override void Up()
        {
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PickedModels", "TimeId", "dbo.TimeModels");
            DropForeignKey("dbo.PickedModels", "TableId", "dbo.TableModels");
            DropIndex("dbo.PickedModels", new[] { "TimeId" });
            DropIndex("dbo.PickedModels", new[] { "TableId" });
            DropTable("dbo.PickedModels");
        }
    }
}

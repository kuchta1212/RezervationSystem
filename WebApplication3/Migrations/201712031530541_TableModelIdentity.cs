namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TableModelIdentity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PickedModels", "TableId", "dbo.TableModels");
            DropForeignKey("dbo.ReservationModels", "TableId", "dbo.TableModels");
            DropPrimaryKey("dbo.TableModels");
            AlterColumn("dbo.TableModels", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TableModels", "Id");
            AddForeignKey("dbo.PickedModels", "TableId", "dbo.TableModels", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ReservationModels", "TableId", "dbo.TableModels", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReservationModels", "TableId", "dbo.TableModels");
            DropForeignKey("dbo.PickedModels", "TableId", "dbo.TableModels");
            DropPrimaryKey("dbo.TableModels");
            AlterColumn("dbo.TableModels", "Id", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TableModels", "Id");
            AddForeignKey("dbo.ReservationModels", "TableId", "dbo.TableModels", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PickedModels", "TableId", "dbo.TableModels", "Id", cascadeDelete: true);
        }
    }
}

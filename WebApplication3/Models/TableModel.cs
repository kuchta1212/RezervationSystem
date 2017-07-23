using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ReservationSystem.Models
{
    public class TableModel
    {
        public TableModel() { }

        public TableModel(int number)
        {
            this.Number = number;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int Number { get; private set; }



    }

//    public class TableDbContext : DbContextWrap
//    {
//        public DbSet<TableModel> Tables { get; set; }
//
//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            //for (int i = 1; i < 10; i++)
//            //    this.Tables.Add(new TableModel(i));
//
//            //this.SaveChanges();
//
//            base.OnModelCreating(modelBuilder);
//        }
//    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ReservationSystem.Models
{
    public class TimeModel
    {
        public TimeModel() { }

        public TimeModel(int starthour, bool isHalf)
        {
            this.StartTime = new TimeSpan(starthour, isHalf ? 30 : 0, 0);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public TimeSpan StartTime { get; private set; }
    }

    public class TimeDbContext : DbContextWrap
    {
        public DbSet<TimeModel> Times { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //for (int i = 18; i < 22; i++)
            //{
            //    this.Times.Add(new TimeModel(i, false));
            //    this.Times.Add(new TimeModel(i, true));
            //}

            //this.SaveChanges();

            base.OnModelCreating(modelBuilder);
        }
    }
}
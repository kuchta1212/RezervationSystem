using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace ReservationSystem.Models
{
    public class DbContextWrap : DbContext
    {
        public DbSet<TableModel> Tables { get; set; }
        public DbSet<ReservationModel> Reservations { get; set; }
        public DbSet<TimeModel> Times { get; set; }
        public DbSet<PickedModel> PickedReservations { get; set; }
        public DbSet<CancelledDayModel> CancelledDays { get; set; }
        public DbSet<SettingModel> Setting { get; set; }
        public DbSet<WeekDayModel> WeekDays { get; set; }
        public DbSet<DateRangeModel> DateRanges { get; set; }

        public DbContextWrap() : base(ConfigurationManager.AppSettings.Get("databaseName"))
        {
          
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DbContextWrap>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}
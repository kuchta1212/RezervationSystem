using System;
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

        public DbContextWrap() : base("name=ReservationSystem")
        {
          
        }
    }
}
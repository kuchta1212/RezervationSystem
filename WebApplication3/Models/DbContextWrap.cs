using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ReservationSystem.Models
{
    public class DbContextWrap : DbContext
    {
        public DbContextWrap() : base("name=ReservationSystem")
        { }
    }
}
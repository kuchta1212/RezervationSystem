using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ReservationSystem.Models
{
    public class ReservationModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //public string UserId { get; set; }

        private DateTime _date;
        [DataType(DataType.Date)]
        public DateTime Date
        {
            get {
                if (_date != null)
                    return _date;
                else
                    return DateTime.Now;
                }
            set { _date = value; }
        }
    }

    public class ReservationDbContext : DbContextWrap
    {
        public DbSet<ReservationModel> Reservations { get; set; }
    }
}
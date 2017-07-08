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

        public string UserId { get; set; }

        [ForeignKey("Table")]
        public int TableId { get; set; }
        public virtual TableModel Table { get; set; }

        [ForeignKey("Time")]
        public int TimeId { get; set; }
        public virtual TimeModel Time { get; set; }

        /*IN HALF HOURS*/
        public int Length { get; set; }

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
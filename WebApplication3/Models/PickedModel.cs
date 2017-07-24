using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ReservationSystem.Models
{
    public class PickedModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }

        //[ForeignKey("Table")]
        public int TableId { get; set; }
        //public virtual TableModel Table { get; set; }

        //[ForeignKey("Time")]
        public int TimeId { get; set; }
        //public virtual TimeModel Time { get; set; }

        /*IN HALF HOURS*/
        public int Length { get; set; }

        [DataType(DataType.Date)]
        public DateTime PickedDate { get; set; }
        
    }

    public class PickedReservationDbContext : DbContextWrap
    {
        public DbSet<PickedModel> PickedReservations { get; set; }
    }
}
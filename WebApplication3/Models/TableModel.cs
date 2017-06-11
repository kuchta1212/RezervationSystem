using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ReservationSystem.Models
{
    public class TableModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Number { get; set; }

        public class TableDbContext : DbContextWrap
        {
            public DbSet<TableModel> Tables { get; set; }
        }
    }
}
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public TimeSpan StartTime { get; private set; }
    }
}
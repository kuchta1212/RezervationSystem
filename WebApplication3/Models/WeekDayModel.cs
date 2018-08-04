using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReservationSystem.Models
{
    public class WeekDayModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [ForeignKey("StartTimeKey")]
        public int StartTime { get; set; }
        public virtual TimeModel StartTimeKey { get; set; }

        [ForeignKey("EndTimeKey")]
        public int EndTime { get; set; }
        public virtual TimeModel EndTimeKey { get; set; }

        [ForeignKey("DateRangeKey")]
        public int DateRange { get; set; }
        public virtual DateRangeModel DateRangeKey { get; set; }

        public bool IsCancelled { get; set; }

    }
}
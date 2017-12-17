using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReservationSystem.Models;

namespace ReservationSystem.Utils
{
    public class WeekDaysView
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public WeekDaysView(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int StartTimeId { get; set; }

        public TimeSpan StartTimeValue { get; set; }

        public int EndTimeId { get; set; }

        public TimeSpan EndTimeValue { get; set; }

        public bool IsCancelled { get; set; }

        public List<TimeModel> Times { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReservationSystem.Models;
using ReservationSystem.Utils;

namespace ReservationSystem.Reservation
{
    public class ReservationView
    {
        public List<TimeModel> Times { get; set; }

        public List<TableModel> Tables { get; set; }

        public DayReservation Day { get; set; }

        public int DateDiff { get; set; }

        public ReturnCode ReturnCode { get; set; } 

        public string ErrorMessage { get; set; }
    }
}
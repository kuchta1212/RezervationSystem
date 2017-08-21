using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReservationSystem.Models;

namespace ReservationSystem.Reservation
{
    public class GroupReservation
    {
        public List<TimeModel> StartTimeId { get; set; }

        public int EndTimeId { get; set; }

        public List<int> TablesId { get; set; }
    }
}
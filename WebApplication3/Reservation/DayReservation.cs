using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationSystem.Reservation
{
    public class DayReservation
    {
        private List<TableReservation> reservations;

        public DayReservation()
        {
            reservations = new List<TableReservation>();
        }


        public void Add(TableReservation tr)
        {
            this.reservations.Add(tr);
        }

    }
}
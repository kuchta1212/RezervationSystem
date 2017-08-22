using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationSystem.Reservation
{
    public class EditReservationView
    {
        public int Id { get; set; }

        public int Table { get; set; }

        public TimeSpan Time { get; set; }

        public string SDate => this.Date.Date.ToString("dd.MM.yyyy");

        public DateTime Date { private get; set; }
    }
}
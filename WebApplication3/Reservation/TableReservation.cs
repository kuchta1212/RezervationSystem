using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReservationSystem.Models;

namespace ReservationSystem.Reservation
{
    public class TableReservation
    {
        private TableModel Table;
        private List<ReservationModel> Reservation;

        public TableReservation(TableModel table)
        {
            this.Table = table;
            this.Reservation = new List<ReservationModel>();
        }

        public void AddReservation(ReservationModel model)
        {
            this.Reservation.Add(model);
        }

    }
}
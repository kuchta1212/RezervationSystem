using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReservationSystem.Models;

namespace ReservationSystem.Reservation
{
    public class TableReservation
    {
        private TableModel table;
        private Dictionary<int, ReservationModel> reservation;
        private Dictionary<int, PickedModel> picked; 

        public TableReservation(TableModel table)
        {
            this.table = table;
            this.reservation = new Dictionary<int, ReservationModel>();
            this.picked = new Dictionary<int, PickedModel>();
        }

        public void AddReservation(ReservationModel model)
        {
            this.reservation.Add(model.TimeId, model);
        }

        public void AddPicked(PickedModel model)
        {
            this.picked.Add(model.TimeId, model);
        }

        public bool IsReservation(int timeId)
        {
            return reservation.ContainsKey(timeId);
        }

        public bool IsPicked(int timeId)
        {
            return picked.ContainsKey(timeId);
        }

        public int TableNumber => this.table.Number;
    }
}
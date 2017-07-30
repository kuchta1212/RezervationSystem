﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReservationSystem.Models;

namespace ReservationSystem.Reservation
{
    public class DayReservation
    {
        private Dictionary<int,TableReservation> reservations;

        public DayReservation()
        {
            reservations = new Dictionary<int, TableReservation>();
        }


        public void Add(TableReservation tr)
        {
            this.reservations.Add(tr.TableNumber, tr);
        }

        public int IsReserved(TimeModel time, TableModel table)
        {
            var tableReservation = reservations[table.Number];
            if (tableReservation.IsReservation(time.Id))
                return 3;
            else if(tableReservation.IsPicked(time.Id))
                return 2;
            else
                return 1;
        }
    }
}
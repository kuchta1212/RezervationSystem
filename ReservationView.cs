using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Components.DictionaryAdapter;
using ReservationSystem.Models;
using ReservationSystem.Utils;

namespace ReservationSystem.Reservation
{
    public class ReservationView : IComparable
    {
        public List<TimeModel> Times { get; set; }

        public List<TableModel> Tables { get; set; }

        public DayReservation Day { get; set; }

        public DateTime Date { get; set; }

        public ReturnCode ReturnCode { get; set; } 

        public bool IsPicked { get; set; }

        public ReservationView()
        {
            //default values
            Date = DateTime.Now;
            Times = new List<TimeModel>();
            Tables = new EditableList<TableModel>();
            Day = new DayReservation();
            ReturnCode = new ReturnCode(ReturnCodeLevel.RELOAD, Resource.ReloadOK, null);
            IsPicked = false;
        }

        public int CompareTo(object o)
        {
           var val = (ReservationView)o;
            if (val.Date <= this.Date)
                return 1;
            return -1;
        }
    }
}
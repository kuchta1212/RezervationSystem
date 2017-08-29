using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Castle.Core.Internal;
using ReservationSystem.Utils;

namespace ReservationSystem.Reservation
{
    public class ReservationPrintable
    {

        public ReservationPrintable()
        {
            this.Days = new SortedList<ReservationView, DateTime>();
        }

        public DateTime Date { get; set; }

        public bool IsWeekly { get; set; }

        public bool Loaded { get; set; }

        public SortedList<ReservationView, DateTime> Days { get; private set; }

        public void AddDay(ReservationView view)
        {
            this.Days.Add(view, view.Date);
        }

        public Dictionary<string, MyUser> Users { get; private set; }

        public void AddUsers(Dictionary<string, MyUser> list)
        {
            if (Users.IsNullOrEmpty())
                Users = list;
            else
            {
                foreach (var user in list.Where(user => !Users.ContainsKey(user.Key)))
                {
                    Users.Add(user.Key, user.Value);
                }
            }
        }
    }
}
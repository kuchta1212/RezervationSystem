using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    public class ReservationController : Controller
    {
        private static ReservationDbContext reservationDbContext = new ReservationDbContext();
        private static TableDbContext tableDbContext = new TableDbContext();
        private static TimeDbContext timeDbContext = new TimeDbContext();

        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save()
        {
            return View("ReservationDone");
        }

        public static List<TableModel> GetTables()
        {
            return tableDbContext.Tables.ToList();
        }

        public static List<TimeModel> GetTimes()
        {
            return timeDbContext.Times.ToList();
        }

        public static DayReservation GetReservationsForDate(DateTime date)
        {
            var reservations = reservationDbContext.Reservations.Where(res => res.Date == date.Date).ToList();
            var tables = tableDbContext.Tables.ToList();

            var day = new DayReservation();


            //var list = (from res in reservations
            //            join table in tables
            //            on res.TableId equals table.Id
            //            select new { table, res }).ToList();


            foreach (var table in tables)
            {
                var tr = new TableReservation(table);
                foreach (var reservation in reservations)
                {
                    if (reservation.TableId == table.Id)
                        tr.AddReservation(reservation);
                }
                day.Add(tr);
            }

            return day;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ReservationSystem.Models;

namespace ReservationSystem.Controllers
{
    public class ReservationController : Controller
    {
        private static ReservationDbContext reservationDbContext = new ReservationDbContext();
        private static TableDbContext tableDbContext = new TableDbContext();
        private static TimeDbContext timeDbContext = new TimeDbContext();
        private static PickedReservationDbContext pickedDbContext = new PickedReservationDbContext();

        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save()
        {
            return View("ReservationDone");
        }

        public ActionResult PickedTime(int table, int time, string sdate)
        {
            DateTime date = DateTime.Parse(sdate);
            string userId = User.Identity.GetUserId();

            //if picked then unpicked
            //var picked = new PickedModel()
            //{
            //    UserId = userId,
            //    TableId = table,
            //    TimeId = time
            //};

            //var userPickeds = pickedDbContext.PickedReservations.Where(
            //    pick => pick.TableId == table && pick.TimeId == time).Select(item => item.UserId).ToList();

            pickedDbContext.PickedReservations.ToList();

            return RedirectToAction("Index", "Home");
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
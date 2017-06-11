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

        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save()
        {
            return View("ReservationDone");
        }

        public static ReservationTable GetReservationsForDate(DateTime date)
        { return new ReservationTable(); }
    }
}
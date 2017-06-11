using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(!Request.IsAuthenticated)
                return View();

            try
            {
                //ViewBag.Reservations = ReservationController.GetReservations();
                ViewBag.Date = DateTime.Now.ToString("dd.MM.yyyy");
                ViewBag.ReservationTable = ReservationController.GetReservationsForDate(DateTime.Now);
                ViewBag.Message = "OK";
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
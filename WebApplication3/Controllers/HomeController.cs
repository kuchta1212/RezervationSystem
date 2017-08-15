using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ReservationSystem.Utils;

namespace ReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private IRepository repository;
        private IReservationManager reservationManager;

        private List<TimeModel> times;
        private List<TableModel> tables;

        public HomeController()
        { }

        public HomeController(IRepository repository, IReservationManager reservationManager)
        {
            this.repository = repository;
            this.reservationManager = reservationManager;
        }

        public ActionResult Index(int? code, int? dateDiff)
        {
            if(!Request.IsAuthenticated)
                return View();

            try
            {

                ViewBag.Date = dateDiff ?? 7;

                using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
                {
                    if(tables == null)
                        tables = repository.GetAll<TableModel>(uow).ToList();
                    ViewBag.Tables = tables;
                    if(times == null)
                        times = repository.GetAll<TimeModel>(uow).ToList();
                    ViewBag.Times = times;
                    ViewBag.ReservationDay = reservationManager.GetReservationsForDate(uow, DateTime.Now, tables, User.Identity.GetUserId());
                }
                if (code != null)
                    ViewBag.ReturnCode = (ReturnCode)code;
                else
                    ViewBag.ReturnCode = ReturnCode.RELOAD_PAGE;
            }
            catch (Exception ex)
            {
                ViewBag.ReturnCode = ReturnCode.ERROR;
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }

        [HttpPost]
        public ActionResult DateChange(string date, string other)
        {
            if (date == null)
                return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.RELOAD_PAGE, dateDiff = 1 });
 

            var dif = DateUtil.DateDiff(DateTime.ParseExact(date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture));
            return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.RELOAD_PAGE, dateDiff = dif});
        }
    }
}
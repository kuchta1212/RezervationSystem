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

            var model = new ReservationView();

            try
            {

                model.DateDiff = dateDiff ?? 1;

                using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
                {
                    if(tables == null)
                        tables = repository.GetAll<TableModel>(uow).ToList();
                    model.Tables = tables;
                    if(times == null)
                        times = repository.GetAll<TimeModel>(uow).ToList();
                    model.Times = times;

                    DateTime date;
                    if (dateDiff == null)
                        date = DateTime.Now;
                    else
                        date = DateUtil.DateDiff(dateDiff.Value);

                    model.Day = reservationManager.GetReservationsForDate(uow, date, tables, User.Identity.GetUserId());
                }
                if (code == (int)ReturnCode.RELOAD_PAGE)
                {
                    model.ReturnCode = ReturnCode.RELOAD_PAGE;
                    ModelState.Clear();
                } 
                else if(code != null)
                    model.ReturnCode = (ReturnCode)code;
                else
                    model.ReturnCode = ReturnCode.RELOAD_PAGE;


            }
            catch (Exception ex)
            {
                model.ReturnCode = ReturnCode.ERROR;
                model.ErrorMessage = ex.Message;
            }

            return View(model);
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
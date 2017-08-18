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

        public ActionResult Index(int? code, DateTime? date)
        {
            if(!Request.IsAuthenticated)
                return View();

            var model = new ReservationView();

            try
            {

                //model.Date = dateDiff ?? 1;
                model.Date = date ?? DateTime.Now.Date;

                using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
                {
                    if(tables == null)
                        tables = repository.GetAll<TableModel>(uow).ToList();
                    model.Tables = tables;
                    if(times == null)
                        times = repository.GetAll<TimeModel>(uow).ToList();
                    model.Times = times;

                    model.IsPicked = reservationManager.GetPickedForDateAndUser(uow, model.Date, User.Identity.GetUserId()).Any();
                    model.Day = reservationManager.GetReservationsForDate(uow, model.Date, tables, User.Identity.GetUserId());
                }
                if (code == (int)ReturnCode.RELOAD_PAGE)
                {
                    model.ReturnCode = ReturnCode.RELOAD_PAGE;
                } 
                else if(code != null)
                    model.ReturnCode = (ReturnCode)code;
                else
                    model.ReturnCode = ReturnCode.RELOAD_PAGE;

                ModelState.Clear();
            }
            catch (Exception ex)
            {
                model.ReturnCode = ReturnCode.ERROR;
                model.ErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DateChange(string date)
        {
            if (date == null)
                return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.RELOAD_PAGE, dateDiff = 1 });

            var parsedDate = DateTime.ParseExact(date, "dd/MM", System.Globalization.CultureInfo.InvariantCulture);
            //var dif = DateUtil.DateDiff(DateTime.ParseExact(date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture));
            return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.RELOAD_PAGE, date = parsedDate });
            

        }
    }
}
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
using log4net;

namespace ReservationSystem.Controllers
{

    public class HomeController : Controller
    {
        private IRepository repository;
        private IReservationManager reservationManager;
        private ITimeManager timeManager;

        private List<TimeModel> times;
        private List<TableModel> tables;

        readonly ILog logger = LogManager.GetLogger(typeof (HomeController));

        public HomeController()
        { }

        public HomeController(IRepository repository, IReservationManager reservationManager, ITimeManager timeManager)
        {
            this.repository = repository;
            this.reservationManager = reservationManager;
            this.timeManager = timeManager;
        }

        public ActionResult Index()
        {
            if(!Request.IsAuthenticated)
                return View(new ReservationView());

            return RedirectToAction("MainTable");
        }

        public ActionResult MainTable(string code, DateTime? date)
        {
            var model = new ReservationView();

            try
            {
                model.Date = date ?? DateTime.Now.Date;

                using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
                {
                    if (tables == null)
                        tables = repository.GetAll<TableModel>(uow).ToList();
                    model.Tables = tables;
                    if (times == null)
                    {
                        times = timeManager.GetTimesForDayOfTheWeek(uow, model.Date);
                        if (!times.Any())
                        {
                            times = repository.GetAll<TimeModel>(uow).ToList();
                            model.Day = new DayReservation(true);
                        }
                    }
                    model.Times = times;

                    model.IsPicked = reservationManager.GetPickedForDateAndUser(uow, model.Date, User.Identity.GetUserId()).Any();
                    if (!model.Day.IsCancelled)
                        model.Day = reservationManager.GetReservationsForDate(uow, model.Date, tables, User.Identity.GetUserId());
                }

                model.ReturnCode = ReturnCode.FromString(code) ?? new ReturnCode(ReturnCodeLevel.RELOAD, Resource.ReloadOK, null);

                ModelState.Clear();
            }
            catch (Exception ex)
            {
                model.ReturnCode.Error(ex.Message);
                logger.Error(ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult DateChange(string date)
        {
            if (date == null)
                return RedirectToAction("MainTable", "Home", new { code = new ReturnCode(ReturnCodeLevel.RELOAD, Resource.ReloadOK, null).ToString(), dateDiff = 1 });

            var parsedDate = DateTime.ParseExact(date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

            return RedirectToAction("MainTable", "Home", new { code = new ReturnCode(ReturnCodeLevel.RELOAD, Resource.ReloadOK, null).ToString(), date = parsedDate });
            

        }

       
        public ActionResult Contact()
        {
            return View("Contact");
        }

        public ActionResult ReservationRules()
        {
            return View("ReservationRules");
        }

        public ActionResult UnregistredView()
        {
            return RedirectToAction("MainTable");
        }

        public ActionResult ViewModal()
        {
            return PartialView("_ModalPartial");
        }
    }
}
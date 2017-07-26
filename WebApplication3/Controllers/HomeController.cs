using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult Index()
        {
            if(!Request.IsAuthenticated)
                return View();

            try
            {
                ViewBag.Date = DateTime.Now.ToString("dd.MM.yyyy");
                using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
                {
                    if(tables == null)
                        tables = repository.GetAll<TableModel>(uow).ToList();
                    ViewBag.Tables = tables;
                    if(times == null)
                        times = repository.GetAll<TimeModel>(uow).ToList();
                    ViewBag.Times = times;
                    ViewBag.ReservationTable = reservationManager.GetReservationsForDate(uow, DateTime.Now, tables);
                }
                ViewBag.Message = "OK";
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }
    }
}
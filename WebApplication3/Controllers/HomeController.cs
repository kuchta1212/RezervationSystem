using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Reservation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

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

        public ActionResult Index(string message)
        {
            if(!Request.IsAuthenticated)
                return View();

            try
            {
                ViewBag.ReservationMessage = message ?? string.Empty;

                ViewBag.Date = DateTime.Now;
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
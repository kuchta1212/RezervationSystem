using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Reservation;
using ReservationSystem.Utils;

namespace ReservationSystem.Controllers
{
    public class AdminController : Controller
    {
        private IRepository _repository;
        private IReservationManager _reservationManager;
        public AdminController(IRepository repository, IReservationManager reservationManager)
        {
            this._repository = repository;
            this._reservationManager = reservationManager;
        }

        public ActionResult Reports(string sdate)
        {
            var model = new ReservationView();

            model.Date = sdate == null ? DateTime.Today : DateTime.ParseExact(sdate, "dd.MM.yyyy", CultureInfo.InvariantCulture);

            
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                model.Tables = _repository.GetAll<TableModel>(uow).ToList();
                model.Times = _repository.GetAll<TimeModel>(uow).ToList();
                model.Day = _reservationManager.GetReservationsForDate(uow, model.Date, model.Tables, User.Identity.GetUserId());
                var users = _reservationManager.GetUsersForDate(uow, model.Date);
                model.Users = HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .Users
                    .Where(u => users.Contains(u.Id))
                    .Select(u => new MyUser {Id = u.Id, UserName = u.UserName})
                    .ToList()
                    .ToDictionary(u=>u.Id);
            }
            
            return View("Reports", model);
        }

        public ActionResult Settings()
        {
            return View("Settings");
        }
    }
}

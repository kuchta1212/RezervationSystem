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

        public ActionResult Reports(string sdate, bool? weekly, bool? daily)
        {
            var model = new ReservationPrintable();

            if (daily != null && daily.Value)
            {
                model.Daily = true;
                model.Weekly = false;
            }
            else
            {
                model.Daily = false;
                model.Weekly = true;
            }

            var date = sdate == null ? DateTime.Today : DateTime.ParseExact(sdate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            if (model.Weekly)
            {
                var diff = date.DayOfWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                if (diff < 0)
                    diff += 7;
                date = date.AddDays(-diff);
            }

            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                var days = model.Weekly ? 5 : 1;

                for (var i = 0; i < days; i++)
                {
                    var view = new ReservationView
                    {
                        Tables = _repository.GetAll<TableModel>(uow).ToList(),
                        Times = _repository.GetAll<TimeModel>(uow).ToList(),
                        Date = date.AddDays(i)
                    };
                    view.Day = _reservationManager.GetReservationsForDate(uow, view.Date, view.Tables, User.Identity.GetUserId());

                    var users = _reservationManager.GetUsersForDate(uow, view.Date);
                    model.AddUsers(HttpContext.GetOwinContext()
                                    .GetUserManager<ApplicationUserManager>()
                                    .Users
                                    .Where(u => users.Contains(u.Id))
                                    .Select(u => new MyUser { Id = u.Id, UserName = u.UserName })
                                    .ToList()
                                    .ToDictionary(u => u.Id));
                    model.AddDay(view);
                }
            }
            
            return View("Reports", model);
        }

        public ActionResult Settings()
        {
            return View("Settings");
        }
    }
}

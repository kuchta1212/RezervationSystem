using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Quartz.Util;
using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Reservation;
using ReservationSystem.Utils;

namespace ReservationSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IRepository _repository;
        private IReservationManager _reservationManager;
        public AdminController(IRepository repository, IReservationManager reservationManager)
        {
            this._repository = repository;
            this._reservationManager = reservationManager;
        }

        public ActionResult Reports(string sdate, string[] IsWeekly)
        {
            var model = new ReservationPrintable();

            model.IsWeekly = true;
            if (IsWeekly != null)
                model.IsWeekly = Boolean.Parse(IsWeekly[0]);

            var date = sdate == null ? DateTime.Today : DateTime.ParseExact(sdate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            if (model.IsWeekly)
            {
                var diff = date.DayOfWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                if (diff < 0)
                    diff += 7;
                date = date.AddDays(-diff);
            }
            model.Date = date;

            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                var days = model.IsWeekly ? 5 : 1;

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
                                    .Select(u => new MyUser { Id = u.Id, UserName = u.UserName, Name = u.Name})
                                    .ToList()
                                    .ToDictionary(u => u.Id));
                    model.AddDay(view);
                }
            }
            
            return View("Reports", model);
        }

        public ActionResult Settings()
        {
            var model = new SettingModelView();

            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                var settingModels = _repository.GetAll<SettingModel>(uow).ToList();
                model.Setting = settingModels;
            }

            using (var appContext = new ApplicationDbContext())
            { 
                var userStore = new UserStore<ApplicationUser>(appContext);
                var userManager = new UserManager<ApplicationUser>(userStore);
                model.Users = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users.
                    Select(u => new MyUser {Id = u.Id, Email = u.Email, UserName = u.UserName, Name = u.Name}).ToList();

                foreach (var user in model.Users)
                {
                    user.IsAdmin = userManager.IsInRole(user.Id, "Admin");
                }
            }

            return View("Settings", model);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            SettingModel model = null;
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                model = _repository.GetByKey<SettingModel>(uow, id);
            }
            return PartialView("_EditPartialView", model);
        }

        [HttpPost] // this action takes the viewModel from the modal
        public ActionResult Edit(SettingModel model, string svalue)
        {
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                var m = _repository.GetByKey<SettingModel>(uow, model.Id);
                m.Value = svalue;
                _repository.Update(uow,m);
                uow.SaveChanges();
            }

            return RedirectToAction("Settings");
        }

        public ActionResult ChangeAdmin(string id, bool? WasAdmin)
        {
            using (var appContext = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(appContext);
                var userManager = new UserManager<ApplicationUser>(userStore);
                if (WasAdmin.Value)
                    userManager.RemoveFromRole(id, "Admin");
                else
                    userManager.AddToRole(id, "Admin");

                appContext.SaveChanges();
            }
            return RedirectToAction("Settings");
        }

        public ActionResult CancelReservation()
        {
            var view = new List<EditReservationView>();
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                var reservations = this._reservationManager.GetReservations(uow);
                Dictionary<string, string> users = null;
                using (var appContext = new ApplicationDbContext())
                {
                    users = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().Users
                        .Select(u => new { u.Id, u.Name })
                        .ToDictionary(t => t.Id, t => t.Name);
                }
                view.AddRange(reservations.Select(res => new EditReservationView() { Id = res.Id, Name = users[res.UserId]  ,Date = res.Date, Table = res.Table.Number, Time = res.Time.StartTime }));
            }
            return View("CancellReservations", view);
        }

        public ActionResult Delete(int? id)
        {
            using (var uow = new UnitOfWork(new DbContextWrap()))
            {
                var reservationModel = _repository.Get<ReservationModel, int>(uow, (r => r.Id == id), (r => r.Id)).FirstOrDefault();

                _repository.Delete<ReservationModel>(uow, reservationModel);
                uow.SaveChanges();
            }

            return RedirectToAction("CancelReservation", "Admin");
        }
    }
}

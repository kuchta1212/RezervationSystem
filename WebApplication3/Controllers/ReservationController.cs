using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Reservation;
using ReservationSystem.Utils;

namespace ReservationSystem.Controllers
{
    public class ReservationController : Controller
    {
        private IRepository repository;
        private IReservationManager reservationManager;

        public ReservationController(IReservationManager reservationManager, IRepository repository)
        {
            this.repository = repository;
            this.reservationManager = reservationManager;
        }

        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save(string sdate)
        {
            try
            {
                var date = DateTime.Parse(sdate);
                using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
                {
                    var picked = reservationManager.GetPickedForDateAndUser(uow, date, User.Identity.GetUserId());
                    foreach (var pick in picked)
                    {
                        var model = new ReservationModel()
                        {
                            Date = pick.PickedDate,
                            TableId = pick.TableId,
                            TimeId = pick.TimeId,
                            UserId = pick.UserId
                        };
                        repository.Delete<PickedModel>(uow, pick);
                        repository.Add<ReservationModel>(uow, model);
                    }
                    uow.SaveChanges();
                }
                return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.RESERVATION_SUCCESS, date = date });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.ERROR });
            }

            
        }

        public ActionResult PickedTime(int table, int time, DateTime? sdate)
        {
            DateTime date = sdate.Value;
            string userId = User.Identity.GetUserId();
            ReturnCode returnCode;

            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                //is free
                var isFree =repository.Get<PickedModel, int>(uow, (pick => pick.TableId == table && pick.TimeId == time && pick.UserId != userId), (item => item.Id));
                if (isFree.Any())
                {
                    returnCode = ReturnCode.RELOAD_PAGE_TABLE_ALREADY_PICKED;
                    return RedirectToAction("Index", "Home", new { code = (int)returnCode, date = date });
                }

                var userPickeds = repository.Get<PickedModel, int>(uow,
                    (pick => pick.TableId == table && pick.TimeId == time && pick.UserId == userId),
                    (item => item.Id));
                var pickedModels= userPickeds as IList<PickedModel> ?? userPickeds.Where(pick => pick.PickedDate.Date == date.Date).ToList();
                if (pickedModels.Any())
                {
                    //unpicked
                    foreach (var picked in pickedModels)
                        repository.Delete<PickedModel>(uow, picked);
                }
                else
                {
                    //picked
                    var picked = new PickedModel()
                    {
                        UserId = userId,
                        TableId = table,
                        TimeId = time,
                        PickedDate = date
                    };
                    repository.Add<PickedModel>(uow, picked);
                }

                uow.SaveChanges();
            }

            return RedirectToAction("Index", "Home", new {code=(int)ReturnCode.RELOAD_PAGE, date=date});
        }

        public ActionResult GroupReservations(string name, string date, string startTime, string endTime, IEnumerable<string> tables)
        {
            if (name == null && date == null && startTime == null)
            {
                var view = new ReservationView();
                using (var uow = new UnitOfWork(new DbContextWrap()))
                {
                    view.Tables = repository.GetAll<TableModel>(uow).ToList();
                    view.Times = repository.GetAll<TimeModel>(uow).ToList();
                }
                return View("GroupReservations", view);
            }
            else
            {
                var timeIds = DateUtil.GetTimeIds(startTime, endTime);
                var finalDate = DateTime.Parse(date);
                foreach (var table in tables)
                {
                    //pro kazdy stul
                    foreach (var time in timeIds)
                    {
                        var model = new ReservationModel()
                        {
                            Date = finalDate.Date,
                            TableId = Int32.Parse(table),
                            TimeId = time
                        };
                    }
                }
            }
        }

        public ActionResult EditReservations()
        {
            List<ReservationModel> reservations;
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                reservations = this.reservationManager.GetReservationsForUser(uow, User.Identity.GetUserId());
            }
            return View("EditReservations", reservations);
        }
    }
}
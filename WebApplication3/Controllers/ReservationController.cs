using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
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

        public ActionResult GroupReservations(string date, string startTime, string endTime, IEnumerable<string> tables)
        {
            if (date == null && startTime == null)
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
                var timeIds = DateUtil.GetTimeIds(repository, startTime, endTime);
                var finalDate = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                var models = new List<ReservationModel>();
                foreach (var table in tables)
                {
                    //pro kazdy stul
                    models.AddRange(timeIds.Select(time => new ReservationModel()
                    {
                        Date = finalDate.Date, TableId = Int32.Parse(table), TimeId = time, UserId = User.Identity.GetUserId()
                    }));
                }
                using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
                {
                    foreach (var model in models)
                    {
                        this.repository.Add<ReservationModel>(uow, model);
                    }
                    uow.SaveChanges();
                }
                return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.RESERVATION_SUCCESS, date = date });
            }
        }

        public ActionResult EditReservations()
        {
            var view = new List<EditReservationView>();
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                var reservations = this.reservationManager.GetReservationsForUser(uow, User.Identity.GetUserId());
                view.AddRange(reservations.Select(res => new EditReservationView() {Id = res.Id, Date = res.Date, Table = res.Table.Number, Time = res.Time.StartTime}));
            }
            return View("EditReservations", view);
        }

        public ActionResult Delete(int? id)
        {
            using (var uow = new UnitOfWork(new DbContextWrap()))
            {
                var reservationModel = repository.Get<ReservationModel, int>(uow, (r => r.Id == id), (r => r.Id)).FirstOrDefault();
                repository.Delete<ReservationModel>(uow, reservationModel);
                uow.SaveChanges();
            }

            return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.RESERVATION_SUCCESSFULLY_DELETED });
        }
    }
}
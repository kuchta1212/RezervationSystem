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
                DateTime date = DateTime.Parse(sdate);
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
                return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.RESERVATION_SUCCESS });
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index", "Home", new { code = (int)ReturnCode.ERROR });
            }

            
        }

        public ActionResult PickedTime(int table, int time, DateTime? sdate)
        {
            DateTime date = DateTime.Now.Date;
            // TODO:
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
                int count = pickedModels.Count;
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

     }
}
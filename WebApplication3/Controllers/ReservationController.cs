using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ReservationSystem.Models;
using ReservationSystem.Repository;

namespace ReservationSystem.Controllers
{
    public class ReservationController : Controller
    {
        private IRepository repository;

        public ReservationController()
        {
        }

        public ReservationController(IRepository repository)
        {
            this.repository = repository;
        }

        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save()
        {
            return View("ReservationDone");
        }

        public ActionResult PickedTime(int table, int time, string sdate)
        {
            DateTime date = DateTime.Parse(sdate);
            string userId = User.Identity.GetUserId();

            //if picked then unpicked
            //var picked = new PickedModel()
            //{
            //    UserId = userId,
            //    TableId = table,
            //    TimeId = time
            //};

            //var userPickeds = pickedDbContext.PickedReservations.Where(
            //    pick => pick.TableId == table && pick.TimeId == time).Select(item => item.UserId).ToList();

            //pickedDbContext.PickedReservations.ToList();

            return RedirectToAction("Index", "Home");
        }

        public List<TableModel> GetTables()
        {
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                return repository.GetAll<TableModel>(uow).ToList();
            }
        }

        public List<TimeModel> GetTimes()
        {
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                return repository.GetAll<TimeModel>(uow).ToList();
            }
        }

        public DayReservation GetReservationsForDate(DateTime date)
        {
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
//                var test = uow.Repository.GetAll<ReservationModel>().ToList();

                var rezervations = repository.Get<ReservationModel, int>(
                    uow, (res => res.Date == date.Date), (res => res.Id),
                    SortOrder.Ascending);

                var tables = repository.GetAll<TableModel>(uow);


                var day = new DayReservation();


//                var list = (from res in rezervations
//                            join table in tables
//                        on res.TableId equals table.Id
//                    select new {table, res}).ToList();

                var reservationModels = rezervations as IList<ReservationModel> ?? rezervations.ToList();
                foreach (var table in tables)
                {
                    var tr = new TableReservation(table);
                    foreach (var reservation in reservationModels)
                    {
                        if (reservation.TableId == table.Id)
                            tr.AddReservation(reservation);
                    }
                    day.Add(tr);
                }

                return day;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ReservationSystem.Models;
using ReservationSystem.Repository;

namespace ReservationSystem.Controllers
{
    public class ReservationController : Controller
    {
//        private static ReservationDbContext reservationDbContext = new ReservationDbContext();
//        private static TableDbContext tableDbContext = new TableDbContext();
//        private static TimeDbContext timeDbContext = new TimeDbContext();

        

        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Save()
        {
            return View("ReservationDone");
        }

        public static List<TableModel> GetTables()
        {
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                return uow.Repository.GetAll<TableModel>().ToList();
            }
        }

        public static List<TimeModel> GetTimes()
        {
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                return uow.Repository.GetAll<TimeModel>().ToList();
            }
        }

        public static DayReservation GetReservationsForDate(DateTime date)
        {
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
//                var test = uow.Repository.GetAll<ReservationModel>().ToList();

                var rezervations = uow.Repository.Get<ReservationModel, int>(
                    (res => res.Date == date.Date), (res => res.Id),
                    SortOrder.Ascending);

                var tables = uow.Repository.GetAll<TableModel>();


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
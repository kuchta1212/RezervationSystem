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

        

        

     }
}
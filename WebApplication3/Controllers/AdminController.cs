using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ReservationSystem.Models;
using ReservationSystem.Repository;

namespace ReservationSystem.Controllers
{
    public class AdminController : Controller
    {
        private IRepository _repository;
        public AdminController(IRepository repository)
        {
            this._repository = repository;
        }

        public ActionResult Reports()
        {
            return View("Reports");
        }

        public ActionResult Settings()
        {
            return View("Settings");
        }
    }
}

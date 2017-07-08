﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(!Request.IsAuthenticated)
                return View();

            try
            {
                ViewBag.Date = DateTime.Now.ToString("dd.MM.yyyy");
                ViewBag.Tables = ReservationController.GetTables();
                ViewBag.Times = ReservationController.GetTimes();
                ViewBag.ReservationTable = ReservationController.GetReservationsForDate(DateTime.Now);
                ViewBag.Message = "OK";
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using ReservationSystem.Models;

namespace ReservationSystem.Utils
{
    public class SettingModelView
    {
        public IEnumerable<SettingModel> Setting { get; set; }

        public IEnumerable<MyUser> Users { get; set; }

        public int NumOfTables { get; set; }
    }
}
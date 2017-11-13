using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Utils;

namespace ReservationSystem.MyMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class MyConfiguration : DbMigrationsConfiguration<ReservationSystem.Models.DbContextWrap>
    {
        public MyConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ReservationSystem.Models.DbContextWrap context)
        {




            //   Logger.Instance.WriteToLog("Configuration migration seed","Configuration",LogType.INFO);
            /*
            ApplicationDbContext appContext = new ApplicationDbContext();
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(appContext));
            // Create Admin Role
             string roleName = "Admin";
             IdentityResult roleResult;
            // Check to see if Role Exists, if not create it
             if (!RoleManager.RoleExists(roleName))
             {
                 roleResult = RoleManager.Create(new IdentityRole(roleName));
             }

             appContext.SaveChanges(); 
             */

            //            var setting = new List<SettingModel>
            //            {
            //                new SettingModel() { Name = "Deadline", Value = "16:00:00"}
            //            };


//            var tables = new List<TableModel>
//            {
//                new TableModel(1) {Id = 1},
//                new TableModel(2) {Id = 2},
//                new TableModel(3) {Id = 3},
//                new TableModel(4) {Id = 4},
//                new TableModel(5) {Id = 5},
//                new TableModel(6) {Id = 6},
//                new TableModel(7) {Id = 7},
//                new TableModel(8) {Id = 8},
//                new TableModel(9) {Id = 9},
//            };
//
//            var times = new List<TimeModel>
//            {
//                new TimeModel(18, false) { Id =1 },
//                new TimeModel(18, true) { Id =2 },
//                new TimeModel(19, false) { Id =3 },
//                new TimeModel(19, true) { Id =4 },
//                new TimeModel(20, false) { Id =5 },
//                new TimeModel(20, true) { Id =6 },
//                new TimeModel(21, false) { Id =7 },
//                new TimeModel(21, true) { Id =8 },
//                new TimeModel(22, false) { Id =9 },
//
//            };
//
//
//            context.Tables.AddRange(tables);
//            context.Times.AddRange(times);
//            context.Setting.AddRange(setting);
//            context.SaveChanges();
//            
    


        }
    }
}

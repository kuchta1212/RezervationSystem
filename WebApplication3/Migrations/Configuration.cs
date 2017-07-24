using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Utils;

namespace ReservationSystem.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ReservationSystem.Models.DbContextWrap>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ReservationSystem.Models.DbContextWrap context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

         //   Logger.Instance.WriteToLog("Configuration migration seed","Configuration",LogType.INFO);
         /*
            ApplicationDbContext appContext = new ApplicationDbContext();
            appContext.Roles.Add(new IdentityRole("Admin"));
            appContext.SaveChanges();*/


            var tables = new List<TableModel>
            {
                new TableModel(1) {Id = 1},
                new TableModel(2) {Id = 2},
                new TableModel(3) {Id = 3},
                new TableModel(4) {Id = 4},
                new TableModel(5) {Id = 5},
                new TableModel(6) {Id = 6},
                new TableModel(7) {Id = 7},
                new TableModel(8) {Id = 8},
                new TableModel(9) {Id = 9},
            };

            var times = new List<TimeModel>
            {
                new TimeModel(18, false) { Id =1 },
                new TimeModel(18, true) { Id =2 },
                new TimeModel(19, false) { Id =3 },
                new TimeModel(19, true) { Id =4 },
                new TimeModel(20, false) { Id =5 },
                new TimeModel(20, true) { Id =6 },
                new TimeModel(21, false) { Id =7 },
                new TimeModel(21, true) { Id =8 },
                new TimeModel(22, false) { Id =9 },

            };

            using (var uow = new UnitOfWork(context))
            {
                tables.ForEach( t => uow.Repository.Add<TableModel>(t));
                times.ForEach(t => uow.Repository.Add<TimeModel>(t));
                uow.SaveChanges();
            }
            


        }
    }
}

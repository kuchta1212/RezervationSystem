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
            if (!context.DateRanges.Any())
            {
                var def = new DateRangeModel()
                {
                    Name = "Default",
                    StartDate = new DateTime(2000, 9, 1),
                    EndTime = new DateTime(2000, 3, 31),
                    IsActive = true
                };

                context.DateRanges.Add(def);
                context.SaveChanges();

                var id = context.DateRanges.First().Id;
                foreach (var day in context.WeekDays)
                {
                    day.DateRange = id;
                }

                context.SaveChanges();
            }


            if (!context.WeekDays.Any())
            {
                var t1800 = context.Times.Where(item => item.StartTime == new TimeSpan(18, 0, 0)).First();
                var t2100 = context.Times.Where(item => item.StartTime == new TimeSpan(21, 0, 0)).First();
                var t2000 = context.Times.Where(item => item.StartTime == new TimeSpan(20, 0, 0)).First();
                var t1830 = context.Times.Where(item => item.StartTime == new TimeSpan(18, 30, 0)).First();
                var t2130 = context.Times.Where(item => item.StartTime == new TimeSpan(21, 30, 0)).First();
                

                var monday = new WeekDayModel()
                {
                    Name = DayOfWeek.Monday.ToString(),
                    StartTimeKey = t1800,
                    StartTime =t1800.Id,
                    EndTimeKey = t2100,
                    EndTime = t2100.Id,
                    IsCancelled = false
                };

                var tuesday = new WeekDayModel()
                {
                    Name = DayOfWeek.Tuesday.ToString(),
                    StartTimeKey = t1800,
                    StartTime = t1800.Id,
                    EndTimeKey = t2100,
                    EndTime = t2100.Id,
                    IsCancelled = false
                };

                var wednesday = new WeekDayModel()
                {
                    Name = DayOfWeek.Wednesday.ToString(),
                    StartTimeKey = t1800,
                    StartTime = t1800.Id,
                    EndTimeKey = t2000,
                    EndTime = t2000.Id,
                    IsCancelled = false
                };

                var thursday = new WeekDayModel()
                {
                    Name = DayOfWeek.Thursday.ToString(),
                    StartTimeKey = t1830,
                    StartTime = t1830.Id,
                    EndTimeKey = t2130,
                    EndTime = t2130.Id,
                    IsCancelled = false
                };

                var friday = new WeekDayModel()
                {
                    Name = DayOfWeek.Friday.ToString(),
                    StartTimeKey = t1800,
                    StartTime = t1800.Id,
                    EndTimeKey = t2100,
                    EndTime = t2100.Id,
                    IsCancelled = true
                };

                context.WeekDays.Add(monday);
                context.WeekDays.Add(tuesday);
                context.WeekDays.Add(wednesday);
                context.WeekDays.Add(thursday);
                context.WeekDays.Add(friday);

                context.SaveChanges();
            }
        }
    }
}

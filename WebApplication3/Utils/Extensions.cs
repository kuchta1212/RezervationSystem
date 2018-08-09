using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReservationSystem.Models;

namespace ReservationSystem.Utils
{
    public static class Extensions
    {
        public static bool IsIn(this DateRangeModel dateRange, DateTime day)
        {
            var start = new DateTime(2000, dateRange.StartDate.Month, dateRange.StartDate.Day);
            var end = new DateTime(dateRange.EndTime < dateRange.StartDate ? 2001 : 2000, dateRange.EndTime.Month, dateRange.EndTime.Day);
            var date = new DateTime(2000, day.Month, day.Day);

            if (start <= date && date <= end)
                return true;

            return end.Year == 2001 && (start > date && end >= date.AddYears(1));
        }
    }
}
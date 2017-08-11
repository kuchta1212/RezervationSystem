using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationSystem.Utils
{
    public class DateUtil
    {

        public static int DateDiff(DateTime date)
        {
            var res = (date - DateTime.Now).TotalDays;
            if (res < 0)
                return 0;
            else
                return (int) res;
        }
    }
}
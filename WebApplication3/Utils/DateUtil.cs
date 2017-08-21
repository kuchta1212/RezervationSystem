using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Components.DictionaryAdapter;

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

        public static DateTime DateDiff(int diff)
        {
            return DateTime.Now.AddDays(diff);
        }

        public static List<int> GetTimeIds(string start, string end)
        {
            return new List<int>();
        }
    }
}
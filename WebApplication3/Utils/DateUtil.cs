using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Components.DictionaryAdapter;
using ReservationSystem.Models;
using ReservationSystem.Repository;

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

        public static List<int> GetTimeIds(IRepository repository, string start, string end)
        {
            var startTime = TimeSpan.Parse(start);
            var endTime = TimeSpan.Parse(end);

            var times = new List<TimeSpan> {};
            var half = new TimeSpan(0, 30, 0);

            while (startTime <= endTime)
            {
               times.Add(endTime);
               endTime = endTime - half;
            }

            List<int> ids;
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                ids = repository.Get<TimeModel,int>(uow, (t => times.Contains(t.StartTime)), (t => t.Id)).Select(t=>t.Id).ToList();
            }
            return ids;

        }
    }
}
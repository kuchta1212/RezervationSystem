using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using ReservationSystem.Models;
using ReservationSystem.Repository;

namespace ReservationSystem.Utils
{
    public class TimeManager : ITimeManager
    {
        private readonly IRepository repository;

        public TimeManager(IRepository repository)
        {
            this.repository = repository;
        }

        public List<TimeModel> GetTimesForDayOfTheWeek(IUnitOfWork unitOfWork, DateTime day)
        {
            var dateRangeId = this.GetDateRangeId(unitOfWork, day);
            var dayOfWeek = day.DayOfWeek.ToString();
            var dayModel = repository.Get<WeekDayModel, int>(unitOfWork, item => item.Name.Equals(dayOfWeek) && item.DateRange == dateRangeId, item => item.Id).FirstOrDefault() ??
                           new WeekDayModel() {IsCancelled = true};
            if(dayModel.IsCancelled)
                return new List<TimeModel>();
            var startTime = repository.GetByKey<TimeModel>(unitOfWork, dayModel.StartTime);
            var endTime = repository.GetByKey<TimeModel>(unitOfWork, dayModel.EndTime);

            return repository.Get<TimeModel, TimeSpan>(unitOfWork,
                time => time.StartTime >= startTime.StartTime && time.StartTime <= endTime.StartTime, time => time.StartTime).ToList();
        }

        private int GetDateRangeId(IUnitOfWork uow, DateTime day)
        {
            var ranges = repository.GetAll<DateRangeModel>(uow);
            var selectedRanges = ranges.Where(dr => dr.IsIn(day));
            if (!selectedRanges.Any() || selectedRanges.Count() >= 2)
            {
                return ranges.First().Id;
            }

            return selectedRanges.First().Id;
        }

        
    }
}
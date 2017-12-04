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

        public List<TimeModel> GetTimesForDayOfTheWeek(IUnitOfWork unitOfWork, string dayOfWeek)
        {
            var dayModel = repository.Get<WeekDayModel, int>(unitOfWork, item => item.Name.Equals(dayOfWeek), item => item.Id).First();
            if(dayModel.IsCancelled)
                return new List<TimeModel>();
            var startTime = repository.GetByKey<TimeModel>(unitOfWork, dayModel.StartTime);
            var endTime = repository.GetByKey<TimeModel>(unitOfWork, dayModel.EndTime);

            return repository.Get<TimeModel, TimeSpan>(unitOfWork,
                time => time.StartTime >= startTime.StartTime && time.StartTime <= endTime.StartTime, time => time.StartTime).ToList();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReservationSystem.Models;
using ReservationSystem.Repository;

namespace ReservationSystem.Utils
{
    public interface ITimeManager
    {
        List<TimeModel> GetTimesForDayOfTheWeek(IUnitOfWork unitOfWork, DateTime day);
    }
}

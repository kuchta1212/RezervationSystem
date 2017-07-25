using ReservationSystem.Models;
using ReservationSystem.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Reservation
{
    public interface IReservationManager
    {
        DayReservation GetReservationsForDate(IUnitOfWork unitOfWork, DateTime date, List<TableModel> tables);
    }
}

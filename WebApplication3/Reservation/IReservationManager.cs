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
        DayReservation GetReservationsForDate(IUnitOfWork unitOfWork, DateTime date, List<TableModel> tables, string userId);

        List<PickedModel> GetPickedForDateAndUser(IUnitOfWork unitOfWork, DateTime date, string userId);

        List<ReservationModel> GetReservationsForUser(IUnitOfWork unitOfWork, string userId);

        List<string> GetUsersForDate(IUnitOfWork unitOfWork, DateTime date);

        bool IsAfterDeadline(IUnitOfWork uow, DateTime date);

        void ReleaseAllPickedReservations(string userId);
    }
}

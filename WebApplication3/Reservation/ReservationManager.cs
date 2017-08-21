using ReservationSystem.Models;
using ReservationSystem.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.UI;

namespace ReservationSystem.Reservation
{
    public class ReservationManager : IReservationManager
    {
        private readonly IRepository repository;

        public ReservationManager(IRepository repository)
        {
            this.repository = repository;
        }

        public List<ReservationModel> GetReservationsForUser(IUnitOfWork unitOfWork, string userId)
        {
            return repository.Get<ReservationModel, int>(unitOfWork, (item => item.UserId == userId), (item => item.Id)).ToList();
        }

        public List<PickedModel> GetPickedForDateAndUser(IUnitOfWork unitOfWork, DateTime date, string userId)
        {
            var userPickeds = repository.Get<PickedModel, int>(unitOfWork, (item => item.UserId == userId), (item => item.Id));
            return userPickeds as List<PickedModel> ?? userPickeds.Where(pick => pick.PickedDate.Date == date.Date).ToList();
        }

        public DayReservation GetReservationsForDate(IUnitOfWork unitOfWork, DateTime date, List<TableModel> tables, string userId)
        {
            // get all reservations for date
            var dayReservationList = repository.GetAll<ReservationModel>(unitOfWork).Where(res => res.Date.Date == date.Date).ToList();
          //  var dayReservationList = rezervations as IList<ReservationModel> ?? rezervations.ToList();

            // get all picked tables by the user for date 
            var pickedTableList = GetPickedForDateAndUser(unitOfWork, date, userId);

            var day = new DayReservation();

            foreach (var table in tables)
            {
                var tr = new TableReservation(table);
                foreach (var reservation in dayReservationList.Where(rese => rese.TableId == table.Id))
                        tr.AddReservation(reservation);

                foreach (var reservation in pickedTableList.Where(rese => rese.TableId == table.Id))
                    tr.AddPicked(reservation);


                day.Add(tr);
            }

            return day;

        }

    }
}
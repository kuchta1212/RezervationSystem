using ReservationSystem.Models;
using ReservationSystem.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ReservationSystem.Reservation
{
    public class ReservationManager : IReservationManager
    {
        private IRepository repository;

        public ReservationManager(IRepository repository)
        {
            this.repository = repository;
        }

        public DayReservation GetReservationsForDate(IUnitOfWork unitOfWork, DateTime date, List<TableModel> tables)
        {
            using (IUnitOfWork uow = new UnitOfWork(new DbContextWrap()))
            {
                //                var test = uow.Repository.GetAll<ReservationModel>().ToList();

                var rezervations = repository.Get<ReservationModel, int>(
                    uow, (res => res.Date == date.Date), (res => res.Id),
                    SortOrder.Ascending);

                  var day = new DayReservation();


                //                var list = (from res in rezervations
                //                            join table in tables
                //                        on res.TableId equals table.Id
                //                    select new {table, res}).ToList();

                var reservationModels = rezervations as IList<ReservationModel> ?? rezervations.ToList();
                foreach (var table in tables)
                {
                    var tr = new TableReservation(table);
                    foreach (var reservation in reservationModels)
                    {
                        if (reservation.TableId == table.Id)
                            tr.AddReservation(reservation);
                    }
                    day.Add(tr);
                }

                return day;
            }
        }

    }
}
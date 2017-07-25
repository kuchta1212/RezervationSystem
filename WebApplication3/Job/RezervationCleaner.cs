using System;
using System.Linq;
using Quartz;
using ReservationSystem.Models;
using ReservationSystem.Repository;
using ReservationSystem.Utils;

namespace ReservationSystem.Job
{
    public class RezervationCleaner : IJob
    {
        private IRepository repository;

        public RezervationCleaner(IRepository repository)
        {
            this.repository = repository;
        }

        public void Execute(IJobExecutionContext context)
        {
            Logger.Instance.WriteToLog("Reservation cleaner job started", "ReservationCleaner", LogType.INFO);

            var date = DateTime.Now.Date;

            // delete all rezervations that are older then this date
            //should runned once a day at 23:55
            using (var uow = new UnitOfWork(new DbContextWrap()))
            {
                var toDelete = this.repository.Get<ReservationModel, int>(uow, model => model.Date <= date, model => model.Id).ToList();
                foreach (var item in toDelete)
                    this.repository.Delete<ReservationModel>(uow, item);
                uow.SaveChanges();
            }
        }
    }
}
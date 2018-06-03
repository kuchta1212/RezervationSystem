using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Quartz;
using Quartz.Impl;
using ReservationSystem.Controllers;
using ReservationSystem.Job;
using ReservationSystem.Reservation;
using Repo = ReservationSystem.Repository;

namespace ReservationSystem.Utils
{
    public class IoC : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<Repo.IRepository>().ImplementedBy<Repo.Repository>());

            container.Register(Component.For<EmailController>());

            container.Register(Component.For<IReservationManager>().ImplementedBy<ReservationManager>());

            container.Register(Component.For<ITimeManager>().ImplementedBy<TimeManager>());

            container.Register(Component.For<RezervationCleaner>());


            // scheduler job
            var factory = new StdSchedulerFactory();
            var scheduler = factory.GetScheduler().Result;
            scheduler.JobFactory = new JobFactory(container);
            scheduler.Start();

            var jobDetail = JobBuilder.Create<RezervationCleaner>().Build();
            var trigger =
                TriggerBuilder.Create().WithDailyTimeIntervalSchedule(builder => builder.StartingDailyAt(new TimeOfDay(23, 55))).Build();

            scheduler.ScheduleJob(jobDetail, trigger);
            
        }
    }
}
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.Owin;
using Owin;
using Quartz;
using Quartz.Impl;
using ReservationSystem.Controllers;
using ReservationSystem.Job;
using ReservationSystem.Utils;

[assembly: OwinStartupAttribute(typeof(ReservationSystem.Startup))]
namespace ReservationSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Logger.Instance.WriteToLog("Starting app...","Startup",LogType.INFO);



            IWindsorInstaller ioc = new IoC();
            var container = new WindsorContainer();

            container.Install(ioc);

//            ioc.Install(new WindsorContainer(), new DefaultConfigurationStore());

            ConfigureAuth(app);

            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            var sched = schedulerFactory.GetScheduler();
            sched.Start();

            var jobDetail = JobBuilder.Create<RezervationCleaner>().Build();
            var trigger =
                TriggerBuilder.Create().WithDailyTimeIntervalSchedule(builder => builder.StartingDailyAt(new TimeOfDay(23,55))).Build();

            sched.ScheduleJob(jobDetail, trigger);
        }
    }
}

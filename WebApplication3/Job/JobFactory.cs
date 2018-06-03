using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Windsor;
using Quartz;
using Quartz.Spi;

namespace ReservationSystem.Job
{
    public class JobFactory : IJobFactory
    {
        private readonly IWindsorContainer container;

        public JobFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;

            var job = (IJob)container.Resolve(jobDetail.JobType);
            return job;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
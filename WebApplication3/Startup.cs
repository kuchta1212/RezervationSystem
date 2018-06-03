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
            ConfigureAuth(app);
        }
    }
}

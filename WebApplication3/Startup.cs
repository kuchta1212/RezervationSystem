using Microsoft.Owin;
using Owin;

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

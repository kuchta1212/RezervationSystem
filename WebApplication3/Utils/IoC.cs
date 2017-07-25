using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
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

            container.Register(Component.For<IReservationManager>().ImplementedBy<ReservationManager>());

            container.Register(Component.For<RezervationCleaner>());

            container.Register(Component.For<ReservationController>());

            container.Register(Component.For<HomeController>());


        }
    }
}
using System;

namespace ReservationSystem.Repository
{
    public interface IUnitOfWork : IDisposable
    {

        IRepository Repository { get; }

        void SaveChanges();
    }
}

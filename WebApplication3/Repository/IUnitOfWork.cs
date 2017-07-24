using System;
using ReservationSystem.Models;

namespace ReservationSystem.Repository
{
    public interface IUnitOfWork : IDisposable
    {

        DbContextWrap DbContext { get; }

        void SaveChanges();
    }
}

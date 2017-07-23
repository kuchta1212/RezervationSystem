using System;
using System.Data.Entity.Infrastructure;
using ReservationSystem.Models;

namespace ReservationSystem.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextWrap _dbContext;
        private Repository _repository;

        public UnitOfWork(DbContextWrap dbContext)
        {
            this._dbContext = dbContext;
        }

        public IRepository Repository => _repository ?? (_repository = new Repository(_dbContext));

        public void SaveChanges()
        {
            ((IObjectContextAdapter)_dbContext).ObjectContext.SaveChanges();
        }

        private bool _disposed;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, 
        ///     releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes off the managed and unmanaged resources used.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_disposed)
                return;

            _disposed = true;
        }

    }
}
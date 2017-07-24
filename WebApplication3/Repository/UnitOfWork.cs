using System;
using System.Data.Entity.Infrastructure;
using ReservationSystem.Models;
using ReservationSystem.Utils;

namespace ReservationSystem.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContextWrap _dbContext;

        public UnitOfWork(DbContextWrap dbContext)
        {
            this._dbContext = dbContext;
        }

        public DbContextWrap DbContext
        {
            get
            {
                if (_dbContext != null)
                    return _dbContext;
                throw new Exception("Missing db context");
            }
        }

        public void SaveChanges()
        {
            Logger.Instance.WriteToLog("Saving Db changes", "UnitOfWork",LogType.INFO);
            ((IObjectContextAdapter)_dbContext).ObjectContext.SaveChanges();
        }

        private bool _disposed;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, 
        ///     releasing, or resetting unmanaged resources.
        /// </summary>
        protected void Dispose(bool bDisposing)
        {
            if (!_disposed)
            {
                if (bDisposing)
                {
                    _dbContext?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Close();
        }

    }
}
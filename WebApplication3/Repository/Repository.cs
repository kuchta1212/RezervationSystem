using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using ReservationSystem.Models;

namespace ReservationSystem.Repository
{
    public class Repository : IRepository
    {
        //Private Variables
        private bool bDisposed;
        private DbContextWrap _dbContext;

        #region Constructor
        /// <summary>
        ///     Initializes a new instance of the
        /// <see cref="Repository<TEntity>" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        internal Repository(DbContextWrap contextObj)
        {
            if (contextObj == null)
                throw new ArgumentNullException("context");
            this._dbContext = contextObj;
        }

        #endregion



        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbContext.Set<TEntity>().Add(entity);

        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            string fqen = GetEntityName<TEntity>();

            object originalItem;
            EntityKey key =
            ((IObjectContextAdapter)_dbContext).ObjectContext.CreateEntityKey(fqen, entity);
            if (((IObjectContextAdapter)_dbContext).ObjectContext.TryGetObjectByKey
            (key, out originalItem))
            {
                ((IObjectContextAdapter)_dbContext).ObjectContext.ApplyCurrentValues
                (key.EntitySetName, entity);
            }

        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            _dbContext.Set<TEntity>().Remove(entity);

        }

        public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return GetQuery<TEntity>().AsEnumerable();
        }

        public IQueryable<TEntity> GetQuery<TEntity>
        (Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetQuery<TEntity>().Where(predicate);
        }

        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class
        {
            string entityName = GetEntityName<TEntity>();
            return ((IObjectContextAdapter)_dbContext).
            ObjectContext.CreateQuery<TEntity>(entityName);
        }

        public IEnumerable<TEntity> Get<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TOrderBy>> orderBy, SortOrder sortOrder = SortOrder.Ascending) where TEntity : class
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return GetQuery(criteria)
                    .OrderBy(orderBy)
                    .AsEnumerable();
            }
            return
                GetQuery(criteria)
                    .OrderByDescending(orderBy)
                    .AsEnumerable();
        }




        public TEntity GetByKey<TEntity>(object keyValue) where TEntity : class
        {
            EntityKey key = GetEntityKey<TEntity>(keyValue);

            object originalItem;
            if (((IObjectContextAdapter)_dbContext).
            ObjectContext.TryGetObjectByKey(key, out originalItem))
            {
                return (TEntity)originalItem;
            }

            return default(TEntity);

        }


        protected void Dispose(bool bDisposing)
        {
            if (!bDisposed)
            {
                if (bDisposing)
                {
                    if (null != _dbContext)
                    {
                        _dbContext.Dispose();
                    }
                }
                bDisposed = true;
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


        private EntityKey GetEntityKey<TEntity>(object keyValue) where TEntity : class
        {
            string entitySetName = GetEntityName<TEntity>();
            ObjectSet<TEntity> objectSet =
            ((IObjectContextAdapter)_dbContext).ObjectContext.CreateObjectSet<TEntity>();
            string keyPropertyName = objectSet.EntitySet.ElementType.KeyMembers[0].ToString();
            var entityKey = new EntityKey
            (entitySetName, new[] { new EntityKeyMember(keyPropertyName, keyValue) });
            return entityKey;
        }

        private string GetEntityName<TEntity>() where TEntity : class
        {
            // Thanks to Kamyar Paykhan -
            // http://huyrua.wordpress.com/2011/04/13/
            // entity-framework-4-poco-repository-and-specification-pattern-upgraded-to-ef-4-1/
            // #comment-688
            string entitySetName = ((IObjectContextAdapter)_dbContext).ObjectContext
                .MetadataWorkspace
                .GetEntityContainer(((IObjectContextAdapter)_dbContext).
                    ObjectContext.DefaultContainerName,
                    DataSpace.CSpace)
                .BaseEntitySets.First(bes => bes.ElementType.Name == typeof(TEntity).Name).Name;
            return string.Format("{0}.{1}",
            ((IObjectContextAdapter)_dbContext).ObjectContext.DefaultContainerName,
                entitySetName);
        }
    }
}
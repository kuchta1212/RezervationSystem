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
        #region Constructor
        /// <summary>
        ///     Initializes a new instance of the
        /// <see cref="Repository<TEntity>" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Repository()
        {
        }

        #endregion



        public void Add<TEntity>(IUnitOfWork uow, TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            uow.DbContext.Set<TEntity>().Add(entity);
        }

        public void Update<TEntity>(IUnitOfWork uow, TEntity entity) where TEntity : class
        {
            string fqen = GetEntityName<TEntity>(uow);

            object originalItem;
            EntityKey key =
            ((IObjectContextAdapter)uow.DbContext).ObjectContext.CreateEntityKey(fqen, entity);
            if (((IObjectContextAdapter)uow.DbContext).ObjectContext.TryGetObjectByKey
            (key, out originalItem))
            {
                ((IObjectContextAdapter)uow.DbContext).ObjectContext.ApplyCurrentValues
                (key.EntitySetName, entity);
            }

        }

        public void Delete<TEntity>(IUnitOfWork uow, TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            uow.DbContext.Set<TEntity>().Remove(entity);

        }

        public IEnumerable<TEntity> GetAll<TEntity>(IUnitOfWork uow) where TEntity : class
        {
            return GetQuery<TEntity>(uow).AsEnumerable();
        }

        public IQueryable<TEntity> GetQuery<TEntity>
        (IUnitOfWork uow, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return GetQuery<TEntity>(uow).Where(predicate);
        }

        public IQueryable<TEntity> GetQuery<TEntity>(IUnitOfWork uow) where TEntity : class
        {
            string entityName = GetEntityName<TEntity>(uow);
            return ((IObjectContextAdapter)uow.DbContext).
            ObjectContext.CreateQuery<TEntity>(entityName);
        }

        public IEnumerable<TEntity> Get<TEntity, TOrderBy>(IUnitOfWork uow, Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, TOrderBy>> orderBy, SortOrder sortOrder = SortOrder.Ascending) where TEntity : class
        {
            if (sortOrder == SortOrder.Ascending)
            {
                return GetQuery(uow, criteria)
                    .OrderBy(orderBy)
                    .AsEnumerable();
            }
            return
                GetQuery(uow, criteria)
                    .OrderByDescending(orderBy)
                    .AsEnumerable();
        }




        public TEntity GetByKey<TEntity>(IUnitOfWork uow, object keyValue) where TEntity : class
        {
            EntityKey key = GetEntityKey<TEntity>(uow, keyValue);

            object originalItem;
            if (((IObjectContextAdapter)uow.DbContext).
            ObjectContext.TryGetObjectByKey(key, out originalItem))
            {
                return (TEntity)originalItem;
            }

            return default(TEntity);

        }

        private EntityKey GetEntityKey<TEntity>(IUnitOfWork uow, object keyValue) where TEntity : class
        {
            string entitySetName = GetEntityName<TEntity>(uow);
            ObjectSet<TEntity> objectSet =
            ((IObjectContextAdapter)uow.DbContext).ObjectContext.CreateObjectSet<TEntity>();
            string keyPropertyName = objectSet.EntitySet.ElementType.KeyMembers[0].ToString();
            var entityKey = new EntityKey
            (entitySetName, new[] { new EntityKeyMember(keyPropertyName, keyValue) });
            return entityKey;
        }

        private string GetEntityName<TEntity>(IUnitOfWork uow) where TEntity : class
        {
            // Thanks to Kamyar Paykhan -
            // http://huyrua.wordpress.com/2011/04/13/
            // entity-framework-4-poco-repository-and-specification-pattern-upgraded-to-ef-4-1/
            // #comment-688
            string entitySetName = ((IObjectContextAdapter)uow.DbContext).ObjectContext
                .MetadataWorkspace
                .GetEntityContainer(((IObjectContextAdapter)uow.DbContext).
                    ObjectContext.DefaultContainerName,
                    DataSpace.CSpace)
                .BaseEntitySets.First(bes => bes.ElementType.Name == typeof(TEntity).Name).Name;
            return string.Format("{0}.{1}",
            ((IObjectContextAdapter)uow.DbContext).ObjectContext.DefaultContainerName,
                entitySetName);
        }
    }
}
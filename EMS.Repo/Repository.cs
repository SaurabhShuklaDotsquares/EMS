﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EMS.Data;
using EMS.Data.saral;
using EMS.Data.saralDT;
using EMS.Repo.Search;
using Microsoft.EntityFrameworkCore;

namespace EMS.Repo
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal db_dsmanagementnewContext Context;
        internal db_saralContext SaralContext;
        internal db_saralDTContext SaralDTContext;
        internal DbSet<TEntity> DbSet;
        internal DbSet<TEntity> DbSetSaral;
        internal DbSet<TEntity> DbSetSaralDT;

        public Repository(db_dsmanagementnewContext context, db_saralContext saralContext, db_saralDTContext saralDTContext)
        {
            Context = context;
            SaralContext = saralContext;
            SaralDTContext = saralDTContext;
            DbSetSaral = saralContext.Set<TEntity>();
            DbSetSaralDT = saralDTContext.Set<TEntity>();
            DbSet = context.Set<TEntity>();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public virtual TEntity FindById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual void InsertGraph(TEntity entity)
        {
            DbSet.Add(entity);
            Context.SaveChanges();
        }
        public virtual void InsertGraphSaral(TEntity entity)
        {
            DbSetSaral.Add(entity);
            SaralContext.SaveChanges();
        }
        public virtual void InsertGraphSaralDT(TEntity entity)
        {
            DbSetSaralDT.Add(entity);
            SaralDTContext.SaveChanges();
        }

        public virtual void InsertCollection(List<TEntity> entityCollection)
        {
            DbSet.AddRange(entityCollection);
            Context.SaveChanges();
        }

        public virtual void DeleteCollection(List<TEntity> entityCollection)
        {
            DbSet.RemoveRange(entityCollection);
            Context.SaveChanges();
        }
        public virtual void UpdateCollection(List<TEntity> entityCollection)
        {
            DbSet.UpdateRange(entityCollection);
            Context.SaveChanges();
        }

        public virtual void Update(TEntity entity)
        {

            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }
        public virtual void UpdateSaral(TEntity entity)
        {

            DbSetSaral.Attach(entity);
            SaralContext.Entry(entity).State = EntityState.Modified;
            SaralContext.SaveChanges();
        }
        public virtual void UpdateSaralDT(TEntity entity)
        {

            DbSetSaralDT.Attach(entity);
            SaralDTContext.Entry(entity).State = EntityState.Modified;
            SaralDTContext.SaveChanges();
        }

        public TEntity Update(TEntity dbEntity, TEntity entity)
        {
            Context.Entry(dbEntity).CurrentValues.SetValues(entity);
            Context.Entry(dbEntity).State = EntityState.Modified;
            return dbEntity;
        }
        public TEntity UpdateSaral(TEntity dbEntity, TEntity entity)
        {
            SaralContext.Entry(dbEntity).CurrentValues.SetValues(entity);
            SaralContext.Entry(dbEntity).State = EntityState.Modified;
            return dbEntity;
        }
        public TEntity UpdateSaralDT(TEntity dbEntity, TEntity entity)
        {
            SaralDTContext.Entry(dbEntity).CurrentValues.SetValues(entity);
            SaralDTContext.Entry(dbEntity).State = EntityState.Modified;
            return dbEntity;
        }

        public virtual void Delete(object id)
        {
            var entity = DbSet.Find(id);
            Context.Entry(entity).State = EntityState.Deleted;
            Delete(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);
            Context.SaveChanges();
        }
        public virtual void DeleteSaral(TEntity entity)
        {
            DbSetSaral.Attach(entity);
            DbSetSaral.Remove(entity);
            SaralContext.SaveChanges();
        }
        public virtual void DeleteSaralDT(TEntity entity)
        {
            DbSetSaralDT.Attach(entity);
            DbSetSaralDT.Remove(entity);
            SaralDTContext.SaveChanges();
        }

        public virtual void Insert(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChanges();
        }
        public virtual void InsertSaral(TEntity entity)
        {
            DbSetSaral.Attach(entity);
            SaralContext.Entry(entity).State = EntityState.Added;
            SaralContext.SaveChanges();
        }
        public virtual void InsertSaralDT(TEntity entity)
        {
            DbSetSaralDT.Attach(entity);
            SaralDTContext.Entry(entity).State = EntityState.Added;
            SaralDTContext.SaveChanges();
        }


        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }
        public virtual void SaveChangesSaral()
        {
            SaralContext.SaveChanges();
        }
        public virtual void SaveChangesSaralDT()
        {
            SaralDTContext.SaveChanges();
        }

        public virtual RepositoryQuery<TEntity> Query()
        {
            var repositoryGetFluentHelper =
                new RepositoryQuery<TEntity>(this);

            return repositoryGetFluentHelper;
        }

        public void ChangeEntityState<T>(T entity, ObjectState state) where T : class
        {
            Context.Entry(entity).State = ConvertState(state);
        }
        public void ChangeEntityStateSaral<T>(T entity, ObjectState state) where T : class
        {
            SaralContext.Entry(entity).State = ConvertState(state);
        }
        public void ChangeEntityStateSaralDT<T>(T entity, ObjectState state) where T : class
        {
            SaralDTContext.Entry(entity).State = ConvertState(state);
        }

        public void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, ObjectState state) where T : class
        {
            foreach (T entity in entityCollection.ToList())
            {
                Context.Entry(entity).State = ConvertState(state);
            }
        }

        internal IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            bool trackingEnabled = false,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>>
                includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (includeProperties != null && includeProperties.Count > 0)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null && pageSize > 0)
            {
                if (page.Value > 1)
                {
                    query = query
                  .Skip((page.Value - 1) * pageSize.Value)
                  .Take(pageSize.Value);
                }
                else
                {
                    query = query.Take(pageSize.Value);
                }
            }

            /* Changes By Tabassum in order to resolve the Lazy loading issue removed  .AsNoTracking() */
            return query;
            //return (trackingEnabled ? query : query.AsNoTracking());
            /*End*/
        }
        internal IQueryable<TEntity> GetSaral(
            Expression<Func<TEntity, bool>> filter = null,
            bool trackingEnabled = false,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>>
                includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSetSaral;

            if (includeProperties != null && includeProperties.Count > 0)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null && pageSize > 0)
            {
                if (page.Value > 1)
                {
                    query = query
                  .Skip((page.Value - 1) * pageSize.Value)
                  .Take(pageSize.Value);
                }
                else
                {
                    query = query.Take(pageSize.Value);
                }
            }

            /* Changes By Tabassum in order to resolve the Lazy loading issue removed  .AsNoTracking() */
            return query;
            //return (trackingEnabled ? query : query.AsNoTracking());
            /*End*/
        }
        
        internal IQueryable<TEntity> GetSaralDT(
            Expression<Func<TEntity, bool>> filter = null,
            bool trackingEnabled = false,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>>
                includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSetSaralDT;

            if (includeProperties != null && includeProperties.Count > 0)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null && pageSize > 0)
            {
                if (page.Value > 1)
                {
                    query = query
                  .Skip((page.Value - 1) * pageSize.Value)
                  .Take(pageSize.Value);
                }
                else
                {
                    query = query.Take(pageSize.Value);
                }
            }

            /* Changes By Tabassum in order to resolve the Lazy loading issue removed  .AsNoTracking() */
            return query;
            //return (trackingEnabled ? query : query.AsNoTracking());
            /*End*/
        }

        //----------------------------------------------------------------        
        public virtual PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery)
        {
            IQueryable<TEntity> sequence = DbSet;

            //Applying filters
            sequence = ManageFilters(searchQuery, sequence);

            //Include Properties
            sequence = ManageIncludeProperties(searchQuery, sequence);

            //Resolving Sort Criteria
            //This code applies the sorting criterias sent as the parameter
            sequence = ManageSortCriterias(searchQuery, sequence);

            return GetTheResult(searchQuery, sequence);
        }

        public virtual PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery, out int totalCount)
        {
            IQueryable<TEntity> sequence = DbSet;

            //Applying filters
            sequence = ManageFilters(searchQuery, sequence);

            //Include Properties
            sequence = ManageIncludeProperties(searchQuery, sequence);

            //Resolving Sort Criteria
            //This code applies the sorting criterias sent as the parameter
            sequence = ManageSortCriterias(searchQuery, sequence);

            return GetTheResult(searchQuery, sequence, out totalCount);
        }

        /// <summary>
        /// Executes the query against the repository (database).
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual PagedListResult<TEntity> GetTheResult(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence)
        {
            //Counting the total number of object.
            var resultCount = sequence.Count();

            var result = (searchQuery.Take > 0)
                                ? (sequence.Skip(searchQuery.Skip).Take(searchQuery.Take).ToList())
                                : (sequence.ToList());

            //Debug info of what the query looks like
            //Console.WriteLine(sequence.ToString());

            // Setting up the return object.
            bool hasNext = (searchQuery.Skip <= 0 && searchQuery.Take <= 0) ? false : (searchQuery.Skip + searchQuery.Take < resultCount);
            return new PagedListResult<TEntity>()
            {
                Entities = result,
                HasNext = hasNext,
                HasPrevious = (searchQuery.Skip > 0),
                Count = resultCount
            };
        }

        /// <summary>
        /// Executes the query against the repository (database).
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual PagedListResult<TEntity> GetTheResult(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence, out int totalCount)
        {
            //Counting the total number of object.
            totalCount = sequence.Count();

            var result = (searchQuery.Take > 0)
                                ? (sequence.Skip(searchQuery.Skip).Take(searchQuery.Take).ToList())
                                : (sequence.ToList());

            //Debug info of what the query looks like
            //Console.WriteLine(sequence.ToString());

            // Setting up the return object.
            bool hasNext = (searchQuery.Skip <= 0 && searchQuery.Take <= 0) ? false : (searchQuery.Skip + searchQuery.Take < totalCount);
            return new PagedListResult<TEntity>()
            {
                Entities = result,
                HasNext = hasNext,
                HasPrevious = (searchQuery.Skip > 0),
                Count = totalCount
            };
        }

        /// <summary>
        /// Resolves and applies the sorting criteria of the SearchQuery
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> ManageSortCriterias(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence)
        {
            if (searchQuery.SortCriterias != null && searchQuery.SortCriterias.Count > 0)
            {
                var sortCriteria = searchQuery.SortCriterias[0];
                var orderedSequence = sortCriteria.ApplyOrdering(sequence, false);

                if (searchQuery.SortCriterias.Count > 1)
                {
                    for (var i = 1; i < searchQuery.SortCriterias.Count; i++)
                    {
                        var sc = searchQuery.SortCriterias[i];
                        orderedSequence = sc.ApplyOrdering(orderedSequence, true);
                    }
                }
                sequence = orderedSequence;
            }
            else
            {
                sequence = ((IOrderedQueryable<TEntity>)sequence).OrderBy(x => (true));
            }
            return sequence;
        }

        /// <summary>
        /// Chains the where clause to the IQueriable instance.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> ManageFilters(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence)
        {
            if (searchQuery.Filters != null && searchQuery.Filters.Count > 0)
            {
                foreach (var filterClause in searchQuery.Filters)
                {
                    sequence = sequence.Where(filterClause);
                }
            }
            return sequence;
        }

        /// <summary>
        /// Includes the properties sent as part of the SearchQuery.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> ManageIncludeProperties(SearchQuery<TEntity> searchQuery, IQueryable<TEntity> sequence)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery.IncludeProperties))
            {
                var properties = searchQuery.IncludeProperties.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var includeProperty in properties)
                {
                    sequence = sequence.Include(includeProperty);
                }
            }
            return sequence;
        }

        private EntityState ConvertState(ObjectState state)
        {
            switch (state)
            {
                case ObjectState.Added:
                    return EntityState.Added;
                case ObjectState.Deleted:
                    return EntityState.Deleted;
                case ObjectState.Modified:
                    return EntityState.Modified;
                default:
                    return EntityState.Unchanged;
            }
        }
        public virtual void DeleteBulk(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
            Context.SaveChanges();
        }

        public virtual void SaveBulk(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                DbSet.Attach(entity);
                Context.Entry(entity).State = EntityState.Added;
            }
            Context.SaveChanges();
        }
        public db_dsmanagementnewContext GetDbContext()
        {
            // Context.
            return Context;
        }
        public db_saralContext GetDbContextSaral()
        {
            // Context.
            return SaralContext;
        }
        public db_saralDTContext GetDbContextSaralDT()
        {
            // Context.
            return SaralDTContext;
        }

        public TEntity InsertCallback(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChanges();
            return entity;
        }
    }
}

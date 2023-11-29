using System.Collections.Generic;
using EMS.Data;
using EMS.Data.saral;
using EMS.Data.saralDT;
using EMS.Repo.Search;

namespace EMS.Repo
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity FindById(object id);
        void InsertGraph(TEntity entity);
        void InsertGraphSaral(TEntity entity);
        void InsertGraphSaralDT(TEntity entity);
        void InsertCollection(List<TEntity> entityCollection);
        void DeleteCollection(List<TEntity> entityCollection);
        void Update(TEntity entity);
        void UpdateSaral(TEntity entity);
        void UpdateSaralDT(TEntity entity);
        TEntity Update(TEntity dbEntity, TEntity entity);
        TEntity UpdateSaral(TEntity dbEntity, TEntity entity);
        TEntity UpdateSaralDT(TEntity dbEntity, TEntity entity);
        void Delete(object id);
        void DeleteSaral(TEntity entity);
        void DeleteSaralDT(TEntity entity);
        void Delete(TEntity entity);
        void Insert(TEntity entity);
        void InsertSaral(TEntity entity);
        void InsertSaralDT(TEntity entity);
        void ChangeEntityState<T>(T entity, ObjectState state) where T : class;
        void ChangeEntityStateSaral<T>(T entity, ObjectState state) where T : class;
        void ChangeEntityStateSaralDT<T>(T entity, ObjectState state) where T : class;
        void ChangeEntityCollectionState<T>(ICollection<T> entityCollection, ObjectState state) where T : class;
        RepositoryQuery<TEntity> Query();
        void SaveChanges();
        void SaveChangesSaral();
        void SaveChangesSaralDT();
        void Dispose();

        PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery);
        PagedListResult<TEntity> Search(SearchQuery<TEntity> searchQuery, out int totalCount);
        void SaveBulk(List<TEntity> entitys);
        void DeleteBulk(List<TEntity> entitys);
        db_dsmanagementnewContext GetDbContext();
        db_saralContext GetDbContextSaral();
        db_saralDTContext GetDbContextSaralDT();

        TEntity InsertCallback(TEntity entity);
        void UpdateCollection(List<TEntity> entityCollection);
    }
}

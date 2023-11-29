using System;
using System.Collections;
using EMS.Data;
using EMS.Data.saral;
using EMS.Data.Model;

namespace EMS.Repo
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly db_dsmanagementnewContext _context;
        private readonly db_saralContext _contextSaral;

        private bool _disposed;
        private Hashtable _repositories;

        public UnitOfWork(db_dsmanagementnewContext context, db_saralContext contextSaral)
        {
            _context = context;
            _contextSaral = contextSaral;
        }

        public UnitOfWork()
        {
            _context = new db_dsmanagementnewContext();
            _contextSaral = new db_saralContext();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public void SaveSaral()
        {
            _contextSaral.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    _context.Dispose();
                    _contextSaral.Dispose();
                }

            _disposed = true;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                            .MakeGenericType(typeof(T)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }
    }
}

namespace EMS.Repo
{
    public interface IUnitOfWork
    {
        void Dispose();
        void Save();
        void SaveSaral();
        void Dispose(bool disposing);
        IRepository<T> Repository<T>() where T : class;
    }
}

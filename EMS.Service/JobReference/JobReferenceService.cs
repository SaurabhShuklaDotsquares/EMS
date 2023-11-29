using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using EMS.Core;
namespace EMS.Service
{
    public class JobReferenceService : IJobReferenceService
    {
        #region "Fields"
        private IRepository<JobReference> repoJobReference;

        #endregion

        #region "Cosntructor"
        public JobReferenceService(IRepository<JobReference> _repoJobReference)
        {
            this.repoJobReference = _repoJobReference;
        }


        #endregion

        #region "Dispose"
        public void Dispose()
        {
            if (repoJobReference != null)
            {
                repoJobReference.Dispose();
                repoJobReference = null;
            }
        }
        #endregion
        public JobReference GetJobReferenceById(int? id)
        {
            return repoJobReference.Query()
                   .Filter(x => x.Id == id).Get().FirstOrDefault();
        }
        public List<JobReference> GetJobReferences()
        {
            return repoJobReference.Query().Get()
                .ToList();
        }
        public List<JobReference> GetJobReferenceByPage(out int total, PagingService<JobReference> pagingService)
        {
            return repoJobReference.Query().Filter(pagingService.Filter)
            .OrderBy(pagingService.Sort)
            .GetPage(pagingService.Start, pagingService.Length, out total).ToList();
        }
        public void Delete(JobReference entity)
        {
            if (entity != null)
            {
                repoJobReference.Delete(entity.Id);
                repoJobReference.SaveChanges();
            }
        }
        public void Save(JobReference entity)
        {
            if (entity.Id == 0)
            {
                repoJobReference.ChangeEntityState<JobReference>(entity, ObjectState.Added);
            }
            else
            {
                repoJobReference.ChangeEntityState<JobReference>(entity, ObjectState.Modified);
            }
            repoJobReference.SaveChanges();
        }
    }
}

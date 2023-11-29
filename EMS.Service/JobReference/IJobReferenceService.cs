using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
namespace EMS.Service
{
    public interface IJobReferenceService : IDisposable
    {
        JobReference GetJobReferenceById(int? id);
        List<JobReference> GetJobReferences();
        List<JobReference> GetJobReferenceByPage(out int total, PagingService<JobReference> pagingService);
        void Save(JobReference entity);
        void Delete(JobReference entity);
    }
}

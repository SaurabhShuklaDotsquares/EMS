using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Service
{
    public class ProcessService : IProcessService
    {
        private readonly IRepository<Process> repoProcess;

        public ProcessService(IRepository<Process> _repoProcess)
        {
           repoProcess = _repoProcess ?? throw new ArgumentNullException("_repoProcess");
        }

        public void Dispose()
        {
            if (repoProcess != null)
            {
                repoProcess.Dispose();
            }
        }

        public IEnumerable<Process> GetAllProcess()
        {
            return repoProcess.Query().Filter(x=>x.IsActive).Get();
        }

        public Process GetProcessById(int id)
        {
            return repoProcess.FindById(id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

using EMS.Data;

namespace EMS.Service
{
    public interface IProcessService : IDisposable
    {
        Process GetProcessById(int id);

        IEnumerable<Process> GetAllProcess();

        //Process Save(PILogRequestDto model);

        //Process UpdateApproval(PILogApprovalDto model);

        //Process RollOut(int id, int currentUserId);

        //List<Process> GetLogsByPaging(out int total, PagingService<Process> pagingService);
    }
}

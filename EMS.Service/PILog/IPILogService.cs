using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;

namespace EMS.Service
{
    public interface IPILogService : IDisposable
    {
        PILog GetPILogById(int id);

        PILog Save(PILogRequestDto model);

        PILog UpdateApproval(PILogApprovalDto model);

        PILog RollOut(int id, int currentUserId);

        List<PILog> GetLogsByPaging(out int total, PagingService<PILog> pagingService);

    }
}




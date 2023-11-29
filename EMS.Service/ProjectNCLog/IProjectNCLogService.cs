using System;
using System.Collections.Generic;
using EMS.Data;
using EMS.Dto;
using System.Linq.Expressions;

namespace EMS.Service
{
    public interface IProjectNCLogService : IDisposable
    {
        List<ProjectAuditPA> GetAuditPAs();

        ProjectNCLog GetNCLogById(int id);

        ProjectNCLog GetNCLog(int id);

        ProjectNCLog Save(ProjectNCLogDto model);

        ProjectNCLog UpdateStatus(ProjectNCLogAuditeeDto model);

        ProjectNCLog CloseAudit(ProjectNCLogDto model);

        List<ProjectNCLog> GetLogsByPaging(out int total, PagingService<ProjectNCLog> pagingService);

        List<Project> GetProjecsFromNCLog(Expression<Func<ProjectNCLog, bool>> filters);
    }
}




using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EMS.Service
{
    public class ProjectNCLogService : IProjectNCLogService
    {
        #region "Fields"

        private readonly IRepository<ProjectAuditPA> repoProjectAuditPA;
        private readonly IRepository<ProjectNCLog> repoProjectNCLog;

        #endregion

        #region "Cosntructor"

        public ProjectNCLogService(IRepository<ProjectNCLog> _repoProjectNCLog,
            IRepository<ProjectAuditPA> _repoProjectAuditPA)
        {
            repoProjectAuditPA = _repoProjectAuditPA;
            repoProjectNCLog = _repoProjectNCLog;
        }

        #endregion

        public List<ProjectAuditPA> GetLessonTopics()
        {
            return repoProjectAuditPA.Query()
                .Filter(x => x.Active)
                .OrderBy(o => o.OrderBy(x => x.Name))
                .Get().ToList();
        }

        public ProjectNCLog GetNCLog(int id)
        {
            return repoProjectNCLog.Query()
                .Filter(x => x.Id == id)
                .GetQuerable()
                .FirstOrDefault();
        }

        public ProjectNCLog GetNCLogById(int id)
        {
            return repoProjectNCLog.FindById(id);
        }

        public ProjectNCLog Save(ProjectNCLogDto model)
        {
            ProjectNCLog logEntity = new ProjectNCLog();

            if (model.Id > 0)
            {
                logEntity = GetNCLogById(model.Id);
                if (logEntity == null || logEntity.AuditorUid != model.CurrentUserId || logEntity.Status != (byte)Enums.ProjectAuditStatus.Open)
                {
                    return null;
                }
            }

            logEntity.ProjectId = model.ProjectId;
            logEntity.AuditDate = model.AuditDate.ToDateTime("dd/MM/yyyy").Value;
            logEntity.AuditCycle = model.AuditCycle;
            logEntity.AuditDesc = model.AuditDesc;
            logEntity.AuditeeUid = model.AuditeeUid;
            logEntity.AuditType = model.AuditType;
            logEntity.FollowUpDate = model.FollowUpDate.ToDateTime("dd/MM/yyyy");
            logEntity.ProjectAuditPAId = model.ProjectAuditPAId;
            logEntity.Status = model.Status;
            logEntity.ModifyDate = DateTime.Now;

            if (logEntity.Id == 0)
            {
                logEntity.AuditorUid = model.CurrentUserId;
                logEntity.CreateDate = DateTime.Now;

                repoProjectNCLog.InsertGraph(logEntity);
            }
            else
            {
                repoProjectNCLog.SaveChanges();
            }

            return logEntity;
        }

        public ProjectNCLog UpdateStatus(ProjectNCLogAuditeeDto model)
        {
            var logEntity = GetNCLogById(model.Id);

            if (logEntity != null && logEntity.AuditeeUid == model.CurrentUserId && logEntity.Status == (byte)Enums.ProjectAuditStatus.Open)
            {
                logEntity.AuditAction = model.AuditAction;
                logEntity.RootCause = model.RootCause;
                logEntity.Status = model.Status;
                logEntity.ModifyDate = DateTime.Now;

                if (logEntity.Status == (byte)Enums.ProjectAuditStatus.Completed)
                {
                    logEntity.CompletedDate = model.CompletedDate.ToDateTime("dd/MM/yyyy").Value;
                }

                repoProjectNCLog.SaveChanges();

                return logEntity;
            }

            return null;
        }

        public ProjectNCLog CloseAudit(ProjectNCLogDto model)
        {
            var logEntity = GetNCLogById(model.Id);

            if (logEntity != null && logEntity.AuditorUid == model.CurrentUserId && logEntity.Status == (byte)Enums.ProjectAuditStatus.Completed)
            {
                logEntity.Status = (byte)Enums.ProjectAuditStatus.Closed;
                logEntity.ModifyDate = DateTime.Now;
                logEntity.ClosedDate = model.ClosedDate.ToDateTime("dd/MM/yyyy").Value;

                repoProjectNCLog.SaveChanges();

                return logEntity;
            }

            return null;
        }

        public List<ProjectNCLog> GetLogsByPaging(out int total, PagingService<ProjectNCLog> pagingService)
        {
            return repoProjectNCLog.Query()
                    .Filter(pagingService.Filter)
                    .OrderBy(pagingService.Sort)
                    .Include(x => x.UserLogin)
                    .Include(x => x.UserLogin1)
                    .Include(x => x.Project)
                    .GetPage(pagingService.Start, pagingService.Length, out total)
                    .ToList();
        }

        public List<Project> GetProjecsFromNCLog(Expression<Func<ProjectNCLog, bool>> filters)
        {
            return repoProjectNCLog.Query()
                        .Filter(filters)
                        .GetQuerable()
                        .GroupBy(x => x.ProjectId)
                        .Select(x => x.FirstOrDefault().Project)
                        .ToList();
        }

        public List<ProjectAuditPA> GetAuditPAs()
        {
            return repoProjectAuditPA.Query()
                  .Filter(x => x.Active)
                  .Get().ToList();
        }

        #region "Dispose"

        public void Dispose()
        {
            if (repoProjectNCLog != null)
            {
                repoProjectNCLog.Dispose();
            }
            if (repoProjectAuditPA != null)
            {
                repoProjectAuditPA.Dispose();
            }
        }

        #endregion
    }
}

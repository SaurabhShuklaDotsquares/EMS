using EMS.Core;
using EMS.Data;
using EMS.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EMS.Service
{
    public interface IProjectClosureService
    {
        List<ProjectClosure> GetProjectClosurePaging(out int total, PagingService<ProjectClosure> pagingSerices);

        List<ProjectClosure> GetProjectClosureReportPaging(out int total, PagingService<ProjectClosure> pagingService, Enums.ProjectClosureFilterType? filterType = null);

        List<Project> GetAllProjectsNamewise(int pmid);
        List<Project> GetAllProjectsNamewise(int pmid, int pmuid);
        ProjectClosure Save(ProjectClosureDto model);
        ProjectClosure SaveByUK(ProjectClosureDto model,int UKPMId);
        ProjectClosure Save(ProjectClosure entity);
        ProjectClosure projectClosureFindById(int Id);

        List<ProjectClosure> GetProjectClosureOnDate(int AddedByUid, DateTime NextStartDate);
        List<ProjectClosureDetail> GetProjectClosureDetailOnDate(int AddedByUid, DateTime NextStartDate);
        ProjectClosureDetail SaveDetail(ProjectClousreDetailDto model);
        List<ProjectClosureDetail> GetProjectClosureDetail(int Id);
        Project GetProjectNameById(int Id);
        Preference GetDataByPmuid(int Id);
        void Delete(int Id);
        List<ProjectClosure> GetProjectClosure(Expression<Func<ProjectClosure, bool>> expr);
        ProjectClosure UpdateProjectStatus(ProjectClosureStatusDto model);
        ProjectClosure GetDataByProjectID(int projectID);
        bool UpdateCRMStatus(int id);
        bool UpdateCRMStatus(ProjectClosure projectClosure);

        int GetUnapprovedClosureCount(int pmUid, int baTlId);

        List<AbroadPM> GetAllAbroadPM(string countryId);
        List<ProjectClosureAbroadPm> GetProjectClosureAbroadPMByProjectId(int projectClosureId);
        void DeleteProjectClosureAbroadPMByProjectClosureId(int Id);
        void SaveProjectClosureAbroadPMB(List<ProjectClosureAbroadPm> entity);

        List<ProjectClosure> GetProjectClosedList(int uid, DateTime? startDate, DateTime? endDate, string type);
        
    }
}

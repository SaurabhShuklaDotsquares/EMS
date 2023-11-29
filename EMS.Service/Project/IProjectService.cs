using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface IProjectService : IDisposable
    {
        IQueryable<Project> GetQueriable(Expression<Func<Project, bool>> expression);
        List<UserActivityManageProject> GetUserActivityManageProject(List<int> ProjectIds, int pmuid);
        List<Project> GetProjectListByPmuid(int pmuid, int uid);
        List<Project> GetProjectListByPmuid(int pmuid);

        List<Project> GetProjectWithOtherPMByPmuid(int pmuid);

        bool DeleteProjectOtherPms(List<ProjectOtherPm> Otherpm);
        List<ProjectOtherPm> GetProjectOtherPmsByPmuidAndProjectId(int pmuid, int projectid);

        List<Project> GetProjectList();

        List<HomeProjectListDto> GetProjectListByDeptId(int? deptId, int PMUid, int uId);
        List<HomeProjectListDto> GetProjectListByUser(int empId);
        List<HomeProjectListDto> GetProjectListByUser(int empId, int pmId);

        List<ProjectDeveloper> GetDevelopersByProjectId(int ProjectId);

        List<ProjectDeveloper> GetAllDevelopersByProjectId(int ProjectId);

        List<Project> GetProjectsByPaging(out int total, PagingService<Project> pagingSerices);
        Project GetProjectById(int projectId);
        bool GetProjectStatus(int projectId);
        bool UpdateProjectDeveloper(List<ProjectDeveloper> projectDeveloperList, int ProjectID);
        bool Save(Project project);
        List<BucketModel> GetBucketModels();
        bool CheckClientId(int id);
        List<Client> GetClientData(string[] clientId);
        int? GetProjectPM(int projectId);
        Project GetProjectByCRMId(int CrmId);
        Project GetProjectByCRMId(int CrmId, int pmUid);

        int GetAssignedActualDedicated();

        int GetAssignedCRMDedicated();

        int GetAssignedActualBucket();

        int GetAssignedCRMBucket();

        int GetExtraDevelopers();

        ProjectAdditionalSupport SaveAdditionalSupportRequest(ProjectAdditionalSupport additionalSupport, int[] assignedUserIds);

        ProjectAdditionalSupport GetAdditionalSupportRequest(int requestId);

        List<Project> GetAdditionalSupportProjectList(int? pmuId, int? requestByUid);

        List<ProjectAdditionalSupport> GetAdditionalSupportByPaging(out int total, PagingService<ProjectAdditionalSupport> pagingService);

        List<RunningProjectsDto> RunningProjectByPM(int pmUid, List<int> departments = null);

        List<BonusProjectDeveloperDto> UnassignedActualDeveloperProjects(int pmUid, int[] runningProjects);

        List<RunningProjectWithDeveloperDto> BucketBasedProjects(int pmUid, int[] runningProjects);
        List<int> BucketProjects(List<int> runningProjects);
        List<int> BucketProjectsByPM(int pmUid, List<int> runningProjects);

        List<BonusProjectDeveloperDto> SEOProjects(int pmUid, int[] runningDevelopers);

        List<BonusProjectDeveloperDto> BonusProjects(int pmUid, List<AssignedDeveloperDto> runningDevelopers);

        int GetPendingAdditionalSupportCount(int pmuId, int tlId);
        int TotalDeveloperWorking(int id);

        //newly Added Methods regarding Project Info Changes 

        bool SaveClient(Client virtualDeveloper);

        bool SaveProjectTech(Project_Tech projTech);

        bool SaveProjectDept(Project_Department projDept);

        bool SaveProjectDeveloper(ProjectDeveloper projDev);

        bool SaveProjectEntity(Project proj);

        List<Project_Department> GetProject_DepartmentDataByProjectId(int ProjectId);

        List<Project_Tech> GetDataProjectTechByProjectId(int projectId);

        bool ProjectDeptDeleted(Project project);
        bool ProjectTechDeleted(Project project);
        List<Project> GetProjectsCompByPMUid(int PMUid);
        Client SaveProjectClient(Client entity);
        List<Project> GetTimeSheetProjectListByPmuid(int pmuid, int uid, int roleId);
        int?[] GetDevelopersByCRMID(int crmID);

        Project GetProjectByProjectIdAndPmId(int projectId, int PMUserId);

        Project GetProjectByCRMIdAndPmId(int CrmId);

        bool ProjectDeveloperDeleted(Project project);
        bool ProjectOtherPMDeleted(Project project);

        void UpdateStatus(Project entity);

        void ChangeEntityCollectionStateAsDelete<T>(ICollection<T> entityCollection) where T : class;
        List<Project> GetProjectListByPmuidOnlyforTimeSheet(int pmuid, int uid);
        List<ProjectAdditionalSupport> GetAdditionalSupportInDuration(int uid, DateTime? startDate, DateTime? endDate);
        List<Project> GetProjectListByCurrentUser(List<int?> ProjectId);
        List<Project> GetProjectListByCurrentUserAppraise(List<int> ProjectIds);
    }
}

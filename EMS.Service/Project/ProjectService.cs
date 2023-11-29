using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace EMS.Service
{
    public class ProjectService : IProjectService
    {
        #region "Fields"
        private IRepository<Project> repoProject;

        private IRepository<ProjectOtherPm> repoProjectOtherPm;


        private IRepository<SubProject> repoSubProject;
        private IRepository<ProjectDeveloper> repoProjectDeveloper;
        private IRepository<BucketModel> repoBucketModel;
        private IRepository<Client> repoClient;
        private IRepository<ProjectAdditionalSupport> repoAdditionalSupport;
        private IRepository<UserLogin> repoUserLogin;

        //for project info
        private IRepository<Project_Tech> repoProjectTech;
        private IRepository<Project_Department> repoProjectDept;
        private IRepository<ProjectDeveloper> repoProjectDev;


        #endregion

        #region "Cosntructor"

        public ProjectService(IRepository<Project> _repoProject, IRepository<ProjectDeveloper> _repoProjectDeveloper, IRepository<SubProject> _repoSubProject, IRepository<BucketModel> repoBucketModel, IRepository<Client> repoClient, IRepository<ProjectAdditionalSupport> repoAdditionalSupport, IRepository<UserLogin> _repoUserLogin,
             IRepository<Project_Tech> _repoProjectTech, IRepository<Project_Department> _repoProjectDept,
             IRepository<ProjectDeveloper> _repoProjectDev, IRepository<ProjectOtherPm> _repoProjectOtherPm)
        {
            repoProjectDeveloper = _repoProjectDeveloper;
            repoProject = _repoProject;
            repoSubProject = _repoSubProject;
            this.repoBucketModel = repoBucketModel;
            this.repoClient = repoClient;
            this.repoAdditionalSupport = repoAdditionalSupport;
            repoUserLogin = _repoUserLogin;

            // for project info
            repoProjectTech = _repoProjectTech;
            repoProjectDept = _repoProjectDept;
            repoProjectDev = _repoProjectDev;
            repoProjectOtherPm = _repoProjectOtherPm;

        }

        #endregion

        public IQueryable<Project> GetQueriable(Expression<Func<Project, bool>> expression)
        {
            return repoProject.Query().AsTracking().Filter(expression).GetQuerable();
        }

        public List<UserActivityManageProject> GetUserActivityManageProject(List<int> ProjectIds, int pmuid)
        {
            return repoProject.Query().Filter(x => ProjectIds.Any(a => a == x.ProjectId) && x.PMUid == pmuid).Get().Select(item => new UserActivityManageProject()
            {
                ProjectId = item.ProjectId,
                ProjectStatus = item.Status,
                ProjectRunningDevloper = item.ProjectDevelopers.Count(x => x.WorkStatus == (int)Core.Enums.ProjectDevWorkStatus.Running),
                Created = DateTime.Now
            }).ToList();
        }


        public List<Project> GetProjectListByPmuid(int pmuid, int uid)
        {
            return repoProject.Query()
                .Filter(P => (P.PMUid == pmuid || P.PMUid == null || P.PMUid == 0 || (P.AbroadPMUid.HasValue ? P.AbroadPMUid.Value == uid : false))
                              && P.Status.ToLower() != "d" || P.ProjectOtherPm.Any(x => x.Pmuid == pmuid))
                .Get()
                .OrderBy(T => T.Name)
                .ToList();
        }
        public List<Project> GetProjectListByCurrentUser(List<int?> ProjectIds)
        {
            return repoProject.Query().Filter(x => ProjectIds.Contains(x.ProjectId)).Get().ToList();
                
        }
        public List<Project> GetProjectListByCurrentUserAppraise(List<int> ProjectIds)
        {
            return repoProject.Query().Filter(x => ProjectIds.Contains(x.ProjectId)).Get().ToList();

        }

        public List<Project> GetTimeSheetProjectListByPmuid(int pmuid, int uid, int roleId)
        {

            bool CMMIByPassAllowed = (roleId == (int)Enums.UserRoles.PMO || roleId == (int)Enums.UserRoles.UKPM
                || roleId == (int)Enums.UserRoles.UKBDM || roleId == (int)Enums.UserRoles.Director);
            return repoProject.Query()
                .Filter(P => (P.PMUid == pmuid || P.PMUid == 0 || P.PMUid == null || (P.AbroadPMUid.HasValue ? P.AbroadPMUid.Value == uid : false) || P.ProjectDevelopers.Select(s => s.Uid).Contains(uid))
                              && (P.Status.ToLower() != "d" || P.ProjectOtherPm.Any(po => po.Pmuid == pmuid)) && (CMMIByPassAllowed || P.IsCmmi != true))
                .Get()
                .OrderBy(T => T.Name)
                .ToList();
        }

        public List<Project> GetProjectListByPmuid(int pmuid)
        {
            return repoProject.Query()
                .Filter(P => P.PMUid == pmuid || P.ProjectOtherPm.Any(po => po.Pmuid == pmuid))
                .Get()
                .OrderBy(T => T.Name).ToList();

        }


        public List<Project> GetProjectWithOtherPMByPmuid(int pmuid)
        {
            return repoProject.Query()
               .Filter(P => P.PMUid == pmuid || P.ProjectOtherPm.Any(po => po.Pmuid == pmuid))
               .Get()
              .ToList();
        }

        public List<ProjectOtherPm> GetProjectOtherPmsByPmuidAndProjectId(int pmuid, int projectid)
        {
            return repoProjectOtherPm.Query()
               .Filter(P => P.Pmuid == pmuid && P.ProjectId == projectid)
               .Get()
              .ToList();
        }

        public bool DeleteProjectOtherPms(List<ProjectOtherPm> Otherpm)
        {
            repoProjectOtherPm.DeleteBulk(Otherpm);
            return true;
        }




        public List<Project> GetProjectList()
        {
            return repoProject.Query()
                .Get()
                .OrderBy(T => T.Name).ToList();
        }

        public List<HomeProjectListDto> GetProjectListByDeptId(int? deptId, int PMUid, int uId)
        {
            try
            {
                //List<HomeProjectListDto> lstProject = repoProject.Query().Filter(S => S.PMUid == PMUid && (S.SubProjects.Any(m => m.ProjectId == S.ProjectId) == false) || S.ProjectOtherPm.Any(po => po.Pmuid == PMUid)).Get()
                //    .OrderBy(s => s.Name)
                //     .ToList().Select(T => new
                //     {
                //         T.ProjectId,
                //         Name = Convert.ToString(T.Name.Trim() + " - " + (T.Client != null && !string.IsNullOrEmpty(T.Client.Name) ? T.Client.Name : "") + " [" + T.CRMProjectId + "]")
                //     }).ToList().Select(N => new HomeProjectListDto { Id = (N.ProjectId + ":0"), Name = N.Name }).ToList();

                List<HomeProjectListDto> lstProject = repoProject.Query().Filter(S => S.PMUid == PMUid && (S.SubProjects.Any(m => m.ProjectId == S.ProjectId) == false) || S.ProjectOtherPm.Any(po => po.Pmuid == PMUid)).Get()
                    .OrderBy(s => s.Name)
                     .Select(T => new HomeProjectListDto
                     {
                         Id = (T.ProjectId + ":0"),
                         Name = $"{T.Name} - [{T.CRMProjectId}]"
                     }).ToList();


                //List<HomeProjectListDto> lstSubProject = repoSubProject.Query().Filter(P => P.Project.PMUid == PMUid && P.Project.Project_Department.Any(g => g.DeptID == deptId) == true).Get().OrderBy(P => P.Project.Name).ToList().Select(T => new
                //{
                //    T.Project.ProjectId,
                //    T.SubProjectId,
                //    Name = Convert.ToString(T.Project.Name.Trim() + " (" + T.SubProjectName.Trim() + ")" + " - " + (T.Project.Client != null && !string.IsNullOrEmpty(T.Project.Client.Name) ? T.Project.Client.Name : "") + " [" + T.Project.CRMProjectId + "]")
                //}).ToList().Select(N => new HomeProjectListDto { Id = N.ProjectId.ToString() + ":" + N.SubProjectId.ToString(), Name = N.Name }).ToList();


                List<HomeProjectListDto> lstSubProject = repoSubProject.Query().Filter(P => P.Project.PMUid == PMUid && P.Project.Project_Department.Any(g => g.DeptID == deptId) == true).Get().OrderBy(P => P.Project.Name)
                    .Select(T => new HomeProjectListDto
                    {
                        Id = $"{T.ProjectId}:{T.SubProjectId}",
                        Name = $"{T.Project.Name} ({T.SubProjectName}) - [{T.Project.CRMProjectId}]"
                    }).ToList();


                //    .ToList().Select(T => new
                //{
                //    T.Project.ProjectId,
                //    T.SubProjectId,
                //    Name = Convert.ToString(T.Project.Name.Trim() + " (" + T.SubProjectName.Trim() + ")" + " - " + (T.Project.Client != null && !string.IsNullOrEmpty(T.Project.Client.Name) ? T.Project.Client.Name : "") + " [" + T.Project.CRMProjectId + "]")
                //}).ToList().Select(N => new HomeProjectListDto { Id = N.ProjectId.ToString() + ":" + N.SubProjectId.ToString(), Name = N.Name }).ToList();




                List<HomeProjectListDto> lstAllProjects = lstProject.Concat(lstSubProject).ToList();
                lstAllProjects.Insert(0, new HomeProjectListDto { Id = "-1", Name = "--Select Project--" });

                return lstAllProjects;
            }
            catch (Exception ex)
            {
                List<HomeProjectListDto> lstAllProjects = new List<HomeProjectListDto>();
                lstAllProjects.Insert(0, new HomeProjectListDto { Id = "-1", Name = ex.Message });
                return lstAllProjects;
            }
        }

        public List<HomeProjectListDto> GetProjectListByUser(int empId)
        {
            try
            {
                List<HomeProjectListDto> lstProject = repoProject.Query().Filter(S => (S.SubProjects.Any(m => m.ProjectId == S.ProjectId) == false) && S.ProjectDevelopers.Any(P => P.Uid == empId) == true).Get()
                    .OrderBy(s => s.Name)
                     .ToList().Select(T => new
                     {
                         T.ProjectId,
                         Name = Convert.ToString(T.Name.Trim() + " - " + (T.Client != null && !string.IsNullOrEmpty(T.Client.Name) ? T.Client.Name : "") + " [" + T.CRMProjectId + "]")
                     }).ToList().Select(N => new HomeProjectListDto { Id = (N.ProjectId + ":0"), Name = N.Name }).ToList();

                List<HomeProjectListDto> lstSubProject = repoSubProject.Query().Filter(P => P.Project.ProjectDevelopers.Any(g => g.Uid == empId) == true).Get().OrderBy(P => P.Project.Name).ToList().Select(T => new
                {
                    T.Project.ProjectId,
                    T.SubProjectId,
                    Name = Convert.ToString(T.Project.Name.Trim() + " (" + T.SubProjectName.Trim() + ")" + " - " + (T.Project.Client != null && !string.IsNullOrEmpty(T.Project.Client.Name) ? T.Project.Client.Name : "") + " [" + T.Project.CRMProjectId + "]")
                }).ToList().Select(N => new HomeProjectListDto { Id = N.ProjectId.ToString() + ":" + N.SubProjectId.ToString(), Name = N.Name }).ToList();


                List<HomeProjectListDto> lstAllProjects = lstProject.Concat(lstSubProject).ToList();
                lstAllProjects.Insert(0, new HomeProjectListDto { Id = "-1", Name = "--Select Project--" });

                return lstAllProjects;
            }
            catch (Exception ex)
            {
                List<HomeProjectListDto> lstAllProjects = new List<HomeProjectListDto>();
                lstAllProjects.Insert(0, new HomeProjectListDto { Id = "-1", Name = ex.Message });
                return lstAllProjects;
            }
        }

        public List<HomeProjectListDto> GetProjectListByUser(int empId, int pmId)
        {
            try
            {
                List<HomeProjectListDto> lstProject = repoProject.Query().Filter(S => S.SubProjects.Any(m => m.ProjectId == S.ProjectId) == false &&
                S.ProjectDevelopers.Any(P => P.Uid == empId) && S.PMUid == pmId || S.ProjectOtherPm.Any(po => po.Pmuid == pmId) && S.ActualDevelopers > 0)
                .Get().OrderBy(s => s.Name).ToList()
                .Select(T => new
                {
                    T.ProjectId,
                    Name = Convert.ToString(T.Name.Trim() + " - " + (T.Client != null && !string.IsNullOrEmpty(T.Client.Name) ? T.Client.Name : "") + " [" + T.CRMProjectId + "]")
                }).ToList().Select(N => new HomeProjectListDto { Id = (N.ProjectId + ":0"), Name = N.Name }).ToList();

                List<HomeProjectListDto> lstSubProject = repoSubProject.Query().Filter(P => P.Project.ProjectDevelopers.Any(g => g.Uid == empId) && P.Project.PMUid == pmId)
                .Get().OrderBy(P => P.Project.Name).ToList()
                .Select(T => new
                {
                    T.Project.ProjectId,
                    T.SubProjectId,
                    Name = Convert.ToString(T.Project.Name.Trim() + " (" + T.SubProjectName.Trim() + ")" + " - " + (T.Project.Client != null && !string.IsNullOrEmpty(T.Project.Client.Name) ? T.Project.Client.Name : "") + " [" + T.Project.CRMProjectId + "]")
                }).ToList().Select(N => new HomeProjectListDto { Id = N.ProjectId.ToString() + ":" + N.SubProjectId.ToString(), Name = N.Name }).ToList();


                List<HomeProjectListDto> lstAllProjects = lstProject.Concat(lstSubProject).ToList();
                lstAllProjects.Insert(0, new HomeProjectListDto { Id = "-1", Name = "--Select Project--" });

                return lstAllProjects;
            }
            catch (Exception ex)
            {
                List<HomeProjectListDto> lstAllProjects = new List<HomeProjectListDto>();
                lstAllProjects.Insert(0, new HomeProjectListDto { Id = "-1", Name = ex.Message });
                return lstAllProjects;
            }
        }

        public List<ProjectDeveloper> GetDevelopersByProjectId(int ProjectId)
        {
            return repoProjectDeveloper.Query().Filter(P => P.ProjectId == ProjectId && P.WorkStatus == 12 && P.VD_id != null && P.Project.Status == "R").Get().ToList();
        }

        public List<ProjectDeveloper> GetAllDevelopersByProjectId(int ProjectId)
        {
            return repoProjectDeveloper.Query().Filter(P => P.ProjectId == ProjectId && P.VD_id != null /*&& P.Project.Status == "R"*/).Get().ToList();
        }

        public List<Project> GetProjectsByPaging(out int total, PagingService<Project> pagingService)
        {
            return repoProject.Query().Filter(pagingService.Filter).
                OrderBy(pagingService.Sort).
                GetPage(pagingService.Start, pagingService.Length, out total).
                ToList();
        }

        public Project GetProjectById(int projectId)
        {
            return repoProject.Query().Filter(p => p.ProjectId == projectId).Get().FirstOrDefault();
        }

        public bool GetProjectStatus(int projectId)
        {
            return repoProject.Query().Get().Any(x => x.ProjectId == projectId && x.Status != "C" && x.Status != "H");
        }

        public bool UpdateProjectDeveloper(List<ProjectDeveloper> projectDeveloperList, int ProjectID)
        {

            bool isActualDevExist = false;
            bool isVd_DevExist = false;
            foreach (var vdDev in projectDeveloperList)
            {
                var matchingVdDev = projectDeveloperList.Where(x => x.VD_id.Equals(vdDev.VD_id)).Count();
                if (matchingVdDev > 1)
                {
                    isVd_DevExist = true;
                    break;
                }
            }
            foreach (var AuDev in projectDeveloperList)
            {
                if (AuDev.Uid != null)
                {
                    var matchingAuDev = projectDeveloperList.Where(x => x.Uid.Equals(AuDev.Uid)).Count();
                    if (matchingAuDev > 1)
                    {
                        isActualDevExist = true;
                        break;
                    }
                }
            }
            if (!isActualDevExist && !isVd_DevExist)
            {

                var project = repoProject.FindById(ProjectID);
                if (projectDeveloperList != null && projectDeveloperList.Count > 0)
                {
                    repoProject.ChangeEntityCollectionState(project.ProjectDevelopers, ObjectState.Deleted);

                    project.ProjectDevelopers.Clear();

                    projectDeveloperList.ToList().ForEach(x =>
                    {
                        project.ProjectDevelopers.Add(x);
                    });
                }
                else
                {
                    repoProject.ChangeEntityCollectionState(project.ProjectDevelopers, ObjectState.Deleted);
                    project.ProjectDevelopers.Clear();
                }
                repoProject.SaveChanges();
                return true;

            }
            else
            {

                return false;
            }
        }
        public bool ProjectDeptDeleted(Project project)
        {
            repoProject.ChangeEntityCollectionState(project.Project_Department, ObjectState.Deleted);
            project.Project_Department.Clear();
            repoProject.SaveChanges();
            return true;
        }
        public bool ProjectTechDeleted(Project project)
        {
            repoProject.ChangeEntityCollectionState(project.Project_Tech, ObjectState.Deleted);
            project.Project_Tech.Clear();
            repoProject.SaveChanges();
            return true;
        }






        public bool Save(Project project)
        {
            bool isAlreadyExist = false;
            if (project.CRMProjectId > 0)
                isAlreadyExist = repoProject.Query().Get().Any(p => p.ProjectId != project.ProjectId && p.CRMProjectId == project.CRMProjectId);


            if (!isAlreadyExist)
            {
                if (project.ProjectId == 0)
                {

                    project.AddDate = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("en-gb"));
                    repoProject.InsertGraph(project);
                }
                else
                {

                    var projectEntity = repoProject.FindById(project.ProjectId);
                    ///*Changes by tabassum*/
                    //      repoProject.ChangeEntityCollectionState(projectEntity.Project_Department, ObjectState.Deleted);
                    //    projectEntity.Project_Department.Clear();


                    //    repoProject.ChangeEntityCollectionState(projectEntity.Project_Tech, ObjectState.Deleted);
                    //    projectEntity.Project_Tech.Clear();

                    var projectUpdated = repoProject.Update(projectEntity, project);

                    if (project.Project_Department != null && project.Project_Department.Count > 0)
                    {
                        //repoProject.ChangeEntityCollectionState(projectUpdated.Project_Department, ObjectState.Deleted);
                        //projectUpdated.Project_Department.Clear();
                        project.Project_Department.ToList().ForEach(a =>
                        {
                            projectUpdated.Project_Department.Add(a);
                        });
                    }
                    else
                    {
                        //repoProject.ChangeEntityCollectionState(projectUpdated.Project_Department, ObjectState.Deleted);
                        //projectUpdated.Project_Department.Clear();
                    }
                    if (project.Project_Tech != null && project.Project_Tech.Count > 0)
                    {
                        //repoProject.ChangeEntityCollectionState(projectUpdated.Project_Tech, ObjectState.Deleted);
                        //projectUpdated.Project_Tech.Clear();
                        project.Project_Tech.ToList().ForEach(a =>
                        {
                            projectUpdated.Project_Tech.Add(a);
                        });
                    }
                    else
                    {
                        //repoProject.ChangeEntityCollectionState(projectUpdated.Project_Tech, ObjectState.Deleted);
                        //projectUpdated.Project_Tech.Clear();
                    }

                }
                repoProject.SaveChanges();
                return true;
            }
            return false;
        }

        public List<BucketModel> GetBucketModels()
        {
            return repoBucketModel.Query().Get().Where(x => x.IsActive == true).ToList();
        }

        public ProjectAdditionalSupport SaveAdditionalSupportRequest(ProjectAdditionalSupport additionalSupport, int[] assignedUserIds)
        {
            try
            {
                //additionalSupport.UserLogins.Clear();

                //foreach (var userId in assignedUserIds)
                //{
                //    additionalSupport.UserLogins.Add(repoUserLogin.FindById(userId));
                //}
                additionalSupport.ProjectAdditionalSupportUser.Clear();

                foreach (var userId in assignedUserIds)
                {
                    additionalSupport.ProjectAdditionalSupportUser.Add(new ProjectAdditionalSupportUser { AssignedU = repoUserLogin.FindById(userId) });
                }
                if (additionalSupport.Id == 0)
                {
                    repoAdditionalSupport.InsertGraph(additionalSupport);

                    // Return entity with all navigation properties
                    //return repoAdditionalSupport.Query().Filter(x => x.Id == additionalSupport.Id).AsTracking().Get().FirstOrDefault();
                    return repoAdditionalSupport.Query().Include(q => q.UserLogin1).Include(q => q.UserLogin2).Filter(x => x.Id == additionalSupport.Id).GetQuerable().FirstOrDefault();
                    //return repoAdditionalSupport.FindById(additionalSupport.Id);
                }
                else
                {
                    repoAdditionalSupport.SaveChanges();
                    return additionalSupport;
                }
            }
            catch
            {
                throw;
            }
        }

        public ProjectAdditionalSupport GetAdditionalSupportRequest(int requestId)
        {
            return repoAdditionalSupport.FindById(requestId);
        }

        public List<Project> GetAdditionalSupportProjectList(int? pmuId, int? requestByUid)
        {
            var queryResult = repoAdditionalSupport.Query()
                                .Filter(x => (!pmuId.HasValue || x.Project.PMUid == pmuId || x.Project.ProjectOtherPm.Any(po => po.Pmuid == pmuId)) &&
                                    (!requestByUid.HasValue || requestByUid.Value == x.RequestByUid))
                                .GetQuerable();
            var projectIds = queryResult.Select(x => x.ProjectId).Distinct();

            return queryResult.Where(x => projectIds.Contains(x.ProjectId)).Select(x => x.Project).ToList();
        }

        public List<ProjectAdditionalSupport> GetAdditionalSupportByPaging(out int total, PagingService<ProjectAdditionalSupport> pagingService)
        {
            return repoAdditionalSupport.Query()
                    .Filter(pagingService.Filter)
                    .OrderBy(pagingService.Sort)
                    .Include(x => x.UserLogin1)
                    .GetPage(pagingService.Start, pagingService.Length, out total)
                    .ToList();
        }

        public bool CheckClientId(int id)
        {
            return repoClient.Query().Get().Any(x => x.ClientId.Equals(id));
        }
        public List<Client> GetClientData(string[] clientId)
        {
            return repoClient.Query().Get().Where(P => (!clientId.Contains(P.ClientId.ToString()))).OrderByDescending(P => P.ModifyDate).ToList();
        }
        public int? GetProjectPM(int projectId)
        {
            return repoProject.Query().Filter(x => x.ProjectId == projectId).Get().Select(x => x.PMUid).FirstOrDefault();
        }

        public Project GetProjectByCRMId(int CrmId)
        {
            return repoProject.Query().Filter(p => p.CRMProjectId == CrmId).Get().FirstOrDefault();
        }

        public Project GetProjectByCRMId(int CrmId, int pmUid)
        {
            return repoProject.Query()
                .Filter(p => p.CRMProjectId == CrmId && p.PMUid == pmUid)
                .GetQuerable()
                .FirstOrDefault();
        }

        public int GetAssignedActualDedicated()
        {
            return repoProjectDeveloper.Query().Filter(P => P.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running
                                                               && P.Project.Status == "R"
                                                               && (P.Project.Model.HasValue ? (new[] { "developer/designer only", "blended -pm from indian office", "blended -pm from uk office" }).Contains(P.Project.BucketModel.ModelName.ToLower()) : false)).Get().Count();

        }

        public int GetAssignedCRMDedicated()
        {


            return repoProject.Query().
                Filter(P => P.Status == "R" &&
                (P.Model.HasValue ?
                (new[] { "developer/designer only", "bucket system- blended", "blended -pm from indian office", "blended -pm from uk office" })
                .Contains(P.BucketModel.ModelName.ToLower()) : false)).Get().ToList().Sum(S => S.ActualDevelopers);

        }

        public int GetAssignedActualBucket()
        {
            return repoProjectDeveloper.Query().Filter(P => P.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running
                                                              && P.Project.Status == "R"
                                                              && (P.Project.Model.HasValue ? (new[] { "developer/designer only", "blended -pm from indian office", "blended -pm from uk office" }).Contains(P.Project.BucketModel.ModelName.ToLower()) == false : true)).Get().Count();
        }

        public int GetAssignedCRMBucket()
        {
            return repoProject.Query().Filter(P => P.Status == "R"
                                                              && (P.Model.HasValue ? (new[] { "developer/designer only", "bucket system- blended", "blended -pm from indian office", "blended -pm from uk office" }).Contains(P.BucketModel.ModelName.ToLower()) == false : true)).Get().ToList().Sum(S => S.ActualDevelopers);
        }

        public int GetExtraDevelopers()
        {

            return repoProject.Query().Filter(P => P.Status == "R" && (P.Model.HasValue ? (new[] { "developer/designer only", "bucket system- blended", "blended -pm from indian office", "blended -pm from uk office" }).Contains(P.BucketModel.ModelName.ToLower()) : false)).Get().Sum(s => s.ActualDevelopers) - (repoProjectDeveloper.Query().Filter(P => P.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running && P.UserLogin != null && P.UserLogin.IsActive == true && P.Project.Model.HasValue ? (new[] { "developer/designer only", "blended -pm from indian office", "blended -pm from uk office" }).Contains(P.Project.BucketModel.ModelName.ToLower()) : false).Get().Count());
        }

        public List<RunningProjectsDto> RunningProjectByPM(int pmUid, List<int> departments = null)
        {
            departments = departments ?? new List<int>();

            var bucketSystem = Common.GetExcludedBucketModelIds();
            return repoProject.Query().Filter(P => (pmUid == 0 || pmUid == P.PMUid) &&
                    P.Status == "R" && !P.IsInHouse &&
                    P.Model.HasValue && !bucketSystem.Contains(P.Model.Value))
                    .Include(c => c.ProjectDevelopers)
                    .Get().GroupBy(g => g.PMUid)
                    .Select(g => new RunningProjectsDto
                    {
                        PMUid = g.Key.Value,
                        TotalDevelopers = g.Sum(p => p.ProjectDevelopers.Count(pd => pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running &&
                                                    (!departments.Any() || (pd.UserLogin != null && departments.Contains(pd.UserLogin.DeptId ?? 0))))),
                        UnassignedActualDevelopers = g.Sum(c => c.ProjectDevelopers.Count(pd => pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running && (pd.Uid == null || pd.Uid == 0))),
                        UnassignedProjectIds = g.Where(c => c.ProjectDevelopers.Any(pd => pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running && (pd.Uid == null || pd.Uid == 0)))
                                                .Select(x => x.ProjectId).ToArray()
                    }).ToList();
        }

        public List<BonusProjectDeveloperDto> UnassignedActualDeveloperProjects(int pmUid, int[] runningProjects)
        {
            var bucketSystem = Common.GetExcludedBucketModelIds();
            var projectDevelopers = repoProject.Query().Filter(p => pmUid == p.PMUid &&
                     p.Status == "R" && !p.IsInHouse && runningProjects.Contains(p.ProjectId) &&
                     p.Model.HasValue && !bucketSystem.Contains(p.Model.Value) &&
                     p.ProjectDevelopers.Any(pd => pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running && (pd.Uid == null || pd.Uid == 0)))
                    .Include(c => c.ProjectDevelopers)
                    .Get()
                    .SelectMany(p => p.ProjectDevelopers.Where(pd => pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running && (pd.Uid == null || pd.Uid == 0)));

            return projectDevelopers.Select(x => new BonusProjectDeveloperDto
            {
                VirDevId = x.VD_id ?? 0,
                VirDevName = x.VirtualDeveloper?.VirtualDeveloper_Name,

                ProjectId = x.ProjectId,
                CRMProjectId = x.Project.CRMProjectId,
                ProjectName = x.Project.Name,
                ModelName = x.Project.BucketModel?.ModelName,
            }).OrderBy(x => x.DevName).ToList();
        }

        public List<RunningProjectWithDeveloperDto> BucketBasedProjects(int pmUid, int[] runningProjects)
        {
            var bucketSystem = Common.GetExcludedBucketModelIds();
            return repoProject.Query().Filter(p => pmUid == p.PMUid &&
                    p.Status == "R" && !p.IsInHouse && runningProjects.Contains(p.ProjectId) &&
                    p.Model.HasValue && bucketSystem.Contains(p.Model.Value) &&
                    p.ProjectDevelopers.Any(pd => pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running))
                    .Include(c => c.ProjectDevelopers)
                    .Get()
                    .Select(p => new RunningProjectWithDeveloperDto
                    {
                        ProjectId = p.ProjectId,
                        CRMProjectId = p.CRMProjectId,
                        ProjectName = p.Name,
                        ModelName = p.BucketModel?.ModelName,
                        ProjectDevelopers = p.ProjectDevelopers.Where(x => x.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running)
                        .Select(x => new AssignedDeveloperDto
                        {
                            VirDevId = x.VD_id ?? 0,
                            VirDevName = x.VirtualDeveloper?.VirtualDeveloper_Name,

                            DevId = x.Uid ?? 0,
                            DevName = x.UserLogin?.Name,

                            AddDate = x.AddDate.ToFormatDateString("dd MMM yyyy")
                        }).ToList()
                    }).OrderBy(x => x.ProjectName).ToList();
        }
        public List<int> BucketProjectsByPM(int pmUid, List<int> runningProjects)
        {
            var bucketSystem = new int[] { 3, 4, 13, 15, 16, 17, 59872 };
            return repoProject.Query().Filter(p => pmUid == p.PMUid &&
                    p.Status == "R" && !p.IsInHouse && runningProjects.Contains(p.ProjectId) &&
                    p.Model.HasValue && bucketSystem.Contains(p.Model.Value) &&
                    p.ProjectDevelopers.Any(pd => pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running))
                    .Include(c => c.ProjectDevelopers)
                    .Get()
                    .Select(p => p.ProjectId).ToList();
        }
        public List<int> BucketProjects(List<int> runningProjects)
        {
            var bucketSystem = new int[] { 3, 4, 13, 15, 16, 17, 59872 };
            return repoProject.Query().Filter(p => 
                    p.Status == "R" && !p.IsInHouse && runningProjects.Contains(p.ProjectId) &&
                    p.Model.HasValue && bucketSystem.Contains(p.Model.Value) &&
                    p.ProjectDevelopers.Any(pd => pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running))
                    .Include(c => c.ProjectDevelopers)
                    .Get()
                    .Select(p => p.ProjectId).ToList();
        }
        public List<BonusProjectDeveloperDto> SEOProjects(int pmUid, int[] runningDevelopers)
        {

            return repoProjectDeveloper.Query()
                     .Filter(pd => pd.Uid.HasValue && runningDevelopers.Contains(pd.Uid.Value) &&
                         pd.Project.Status == "R" && pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running &&
                         !pd.Project.IsInHouse && pd.Project.PMUid == pmUid &&
                         pd.Project.Model == (int)Enums.BucketModel.SEO)
                     .Include(pd => pd.Project)
                     .Get()
                     .Select(x => new BonusProjectDeveloperDto
                     {
                         DevId = x.Uid ?? 0,
                         DevName = x.UserLogin?.Name,
                         Department = x.UserLogin?.Department?.Name,

                         VirDevId = x.VD_id ?? 0,
                         VirDevName = x.VirtualDeveloper?.VirtualDeveloper_Name,

                         ProjectId = x.ProjectId,
                         CRMProjectId = x.Project.CRMProjectId,
                         ProjectName = x.Project.Name,
                         ModelName = x.Project.BucketModel?.ModelName
                     })
                     .Where(x => !x.IsSelected)
                     .OrderBy(x => x.DevName).ToList();
        }

        public List<BonusProjectDeveloperDto> BonusProjects(int pmUid, List<AssignedDeveloperDto> runningDevelopers)
        {
            int[] userIds = runningDevelopers.Select(x => x.DevId).ToArray();

            var bucketSystem = Common.GetExcludedBucketModelIds();

            return repoProjectDeveloper.Query()
                     .Filter(pd => pd.Uid.HasValue && userIds.Contains(pd.Uid.Value) &&
                         pd.Project.Status == "R" && pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running &&
                         !pd.Project.IsInHouse && pd.Project.PMUid == pmUid &&
                         pd.Project.Model != (int)Enums.BucketModel.SEO &&
                         !bucketSystem.Contains(pd.Project.Model ?? 0))
                     .Include(pd => pd.Project)
                     .Get()
                     .Select(x => new BonusProjectDeveloperDto
                     {
                         DevId = x.Uid ?? 0,
                         DevName = x.UserLogin?.Name,
                         Department = x.UserLogin?.Department?.Name,

                         VirDevId = x.VD_id ?? 0,
                         VirDevName = x.VirtualDeveloper?.VirtualDeveloper_Name,

                         ProjectId = x.ProjectId,
                         CRMProjectId = x.Project.CRMProjectId,
                         ProjectName = x.Project.Name,
                         ModelName = x.Project.BucketModel?.ModelName,

                         IsSelected = runningDevelopers.Any(d => d.DevId == x.Uid && d.ProjectId == x.ProjectId)
                     })
                     .Where(x => !x.IsSelected)
                     .OrderBy(x => x.DevName).ToList();
        }

        public int GetPendingAdditionalSupportCount(int pmuId, int tlId)
        {
            int count = 0;
            if (tlId > 0)
            {
                count = repoAdditionalSupport.Query()
                                .Filter(x => (x.Status == (byte)Enums.AddSupportRequestStatus.Partial || x.Status == (byte)Enums.AddSupportRequestStatus.Pending) &&
                                        x.Project.PMUid == pmuId &&
                                        (x.TLId == tlId || x.RequestByUid == tlId))
                                .GetQuerable()
                                .Count();
            }
            else
            {
                count = repoAdditionalSupport.Query()
                                .Filter(x => x.Project.PMUid == pmuId &&
                                        (x.Status == (byte)Enums.AddSupportRequestStatus.Partial ||
                                         x.Status == (byte)Enums.AddSupportRequestStatus.Pending))
                                .GetQuerable()
                                .Count();
            }
            return count;
        }
        public int TotalDeveloperWorking(int id)
        {
            return id > 0 ? ((int)repoProject.FindById(id)?.ProjectDevelopers?.Count(d => d.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running)) : 0;
        }

        public bool SaveClient(Client client)
        {
            if (client != null)
            {
                client.AddDate = DateTime.Now;
                repoClient.ChangeEntityState<Client>(client, ObjectState.Added);
                repoClient.SaveChanges();
                return true;
            }
            return false;
        }

        public Client SaveProjectClient(Client entity)
        {
            if (entity != null && entity.ClientId == 0)
            {
                repoClient.Insert(entity);
                return entity;
            }
            else if (entity.ClientId > 0)
            {
                var newentity = new Client();
                newentity = repoClient.FindById(entity.ClientId);
                if (newentity != null)
                {
                    newentity.Name = entity.Name;
                    newentity.ModifyDate = DateTime.Now;
                    newentity.PMUid = entity.PMUid;
                    newentity.IsActive = true;
                    repoClient.ChangeEntityState<Client>(newentity, ObjectState.Modified);
                    repoClient.SaveChanges();
                }
                return null;
            }
            return null;
        }


        public List<Project_Tech> GetDataProjectTechByProjectId(int projectId)
        {
            return repoProjectTech.Query().Filter(x => x.ProjectID == projectId).Get().ToList();
        }

        public bool SaveProjectTech(Project_Tech projTech)
        {
            try
            {
                if (projTech != null)
                {
                    repoProjectTech.Insert(projTech);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool SaveProjectDept(Project_Department projDept)
        {
            try
            {
                if (projDept != null)
                {
                    repoProjectDept.Insert(projDept);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool SaveProjectDeveloper(ProjectDeveloper projDev)
        {
            try
            {
                if (projDev != null)
                {
                    repoProjectDev.Insert(projDev);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public bool SaveProjectEntity(Project proj)
        {
            try
            {
                if (proj != null)
                {
                    if (proj.ProjectId > 0)
                    {
                        repoProject.Update(proj);
                        return true;
                    }
                    else
                    {
                        repoProject.Insert(proj);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<Project_Department> GetProject_DepartmentDataByProjectId(int ProjectId)
        {
            return repoProjectDept.Query().Filter(PG => PG.ProjectID == ProjectId).Get().ToList();
        }

        public int?[] GetDevelopersByCRMID(int crmID)
        {
            return repoProjectDeveloper.Query().Filter(pd => pd.VD_id != null && pd.Project.CRMProjectId == crmID).Get().Distinct().Select(pd => pd.VD_id).ToArray();
        }




        public Project GetProjectByProjectIdAndPmId(int projectId, int PMUserId)
        {
            return repoProject.Query().Filter(p => (p.ProjectId == projectId && p.PMUid == PMUserId)
            || p.ProjectOtherPm.Any(x => x.ProjectId == projectId && x.Pmuid == PMUserId)).Get().FirstOrDefault();
        }

        public List<Project> GetProjectsCompByPMUid(int PMUid)
        {
            return repoProject.Query()
               .Filter(P => P.PMUid == PMUid || P.ProjectOtherPm.Any(po => po.Pmuid == PMUid))
               .Get().OrderByDescending(x => x.ModifyDate)
              .ToList();

            //repoProject.Query().Filter(x => x.PMUid == PMUid).Get().OrderByDescending(x => x.ModifyDate).ToList();

        }

        public Project GetProjectByCRMIdAndPmId(int CrmId)
        {
            return repoProject.Query()
                .Filter(p => p.CRMProjectId == CrmId)
                .GetQuerable()
                .FirstOrDefault();
        }





        public bool ProjectDeveloperDeleted(Project project)
        {
            repoProject.ChangeEntityCollectionState(project.ProjectDevelopers, ObjectState.Deleted);
            project.ProjectDevelopers.Clear();
            repoProject.SaveChanges();
            return true;
        }



        public bool ProjectOtherPMDeleted(Project project)
        {
            repoProject.ChangeEntityCollectionState(project.ProjectOtherPm, ObjectState.Deleted);
            project.ProjectOtherPm.Clear();
            repoProject.SaveChanges();
            return true;
        }

        public void ChangeEntityCollectionStateAsDelete<T>(ICollection<T> entityCollection) where T : class
        {
            repoProject.ChangeEntityCollectionState(entityCollection, ObjectState.Deleted);
        }

        public void UpdateStatus(Project project)
        {
            repoProject.Update(project);
        }

        public List<Project> GetProjectListByPmuidOnlyforTimeSheet(int pmuid, int uid)
        {
            return repoProject.Query()
                .Filter(P => (P.PMUid == pmuid || P.PMUid == 0 || P.PMUid == null || (P.AbroadPMUid.HasValue ? P.AbroadPMUid.Value == uid : false) || P.ProjectDevelopers.Select(s => s.Uid).Contains(uid))
                               && P.IsManualTimeSheetAllowed == true && (P.Status.ToLower() != "d" || P.ProjectOtherPm.Any(po => po.Pmuid == pmuid)) && P.IsCmmi != true)
                .Get()
                .OrderBy(T => T.Name)
                .ToList();
        }

        public List<ProjectAdditionalSupport> GetAdditionalSupportInDuration(int uid, DateTime? startDate, DateTime? endDate)
        {
            var filter = PredicateBuilder.True<ProjectAdditionalSupport>();
            filter = filter.And(x => x.ProjectAdditionalSupportUser.Any(u => u.AssignedUid == uid));

            if (startDate.HasValue && endDate.HasValue)
            {
                filter = filter.And(x => ((x.StartDate >= startDate && x.StartDate <= endDate) // start date fall between specified range
                                    || (x.EndDate >= startDate && x.EndDate <= endDate) // end date fall between specified range
                                    || (x.StartDate <= startDate && x.EndDate >= endDate)// covers date range
                                    || (x.StartDate >= startDate && x.EndDate <= endDate) // Inside specified date range
                                    ));
            }
            else if (startDate.HasValue)
            {
                filter = filter.And(x => x.EndDate >= startDate); // ended after or equal filter start
            }
            else if (endDate.HasValue)
            {
                filter = filter.And(x => x.EndDate >= endDate && x.StartDate <= endDate);// ended after or equal filter end date and started equal or before filter and date
            }

            return repoAdditionalSupport.Query()
                                .Filter(filter)
                                .Get().ToList();
            //return repoAdditionalSupport.Query()
            //                    .Filter(x =>
            //                        x.ProjectAdditionalSupportUser.Any(u=>u.AssignedUid==uid)
            //                        && (( x.StartDate>=startDate && x.StartDate<=endDate) // start date fall between specified range
            //                        || (x.EndDate >= startDate && x.EndDate <= endDate) // end date fall between specified range
            //                        || (x.StartDate <= startDate && x.EndDate >= endDate)// covers date range
            //                        || (x.StartDate >= startDate && x.EndDate <= endDate) // Inside specified date range
            //                        ))
            //                    .Get().ToList();
        }

      

        #region "Dispose"
        public void Dispose()
        {
            if (repoProject != null)
            {
                repoProject.Dispose();
                repoProject = null;
            }

            if (repoProjectDeveloper != null)
            {
                repoProjectDeveloper.Dispose();
                repoProjectDeveloper = null;
            }
        }



        #endregion

    }
}

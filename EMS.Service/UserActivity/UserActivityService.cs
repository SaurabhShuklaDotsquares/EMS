using EMS.Core;
using EMS.Data;

using EMS.Data.Model;
using EMS.Dto;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EMS.Service
{
    public class UserActivityService : IUserActivityService
    {
        #region "Fields"
        private IRepository<UserActivity> repoUserActivity;
        private IRepository<UserActivityLog> repoUserActivityLog;
        private IRepository<UserLogin> repoUserMaster;
        private IRepository<BucketModel> repoBucketModel;
        private IRepository<DomainType> repoDomainType;
        private Microsoft.Extensions.Configuration.IConfiguration configuration;
        #endregion

        #region "Cosntructor"
        public UserActivityService(IRepository<UserActivity> _repoUserActivity, IRepository<UserActivityLog> _repoUserActivityLog, IRepository<UserLogin> _repoUserMaster,
            IRepository<BucketModel> _repoBucketModel, IRepository<DomainType> _repoDomainType, Microsoft.Extensions.Configuration.IConfiguration _configuration)
        {
            repoUserActivity = _repoUserActivity;
            repoUserActivityLog = _repoUserActivityLog;
            repoUserMaster = _repoUserMaster;
            repoBucketModel = _repoBucketModel;
            repoDomainType = _repoDomainType;
            configuration = _configuration;
        }


        #endregion

        #region "Dispose"
        public void Dispose()
        {
            if (repoUserActivity != null)
            {
                repoUserActivity.Dispose();
                repoUserActivity = null;
            }
            if (repoUserMaster != null)
            {
                repoUserMaster.Dispose();
                repoUserMaster = null;
            }
            if (repoBucketModel != null)
            {
                repoBucketModel.Dispose();
                repoBucketModel = null;
            }
        }
        #endregion

        public UserActivity GetUserActivityByUid(int Uid, DateTime? date = null)
        {
            date = date?.Date ?? DateTime.Now.Date;
            var objUserActivity = repoUserActivity.Query()
                                    .Filter(x => x.Uid == Uid && x.UserLogin.IsActive == true && x.Date == date)
                                    .OrderBy(o => o.OrderByDescending(x => x.ActivityID))
                                    .GetQuerable()
                                    .FirstOrDefault();

            return objUserActivity;
        }
        public UserActivity GetUserActivityByUidDesc(int Uid)
        {
            var objUserActivity = repoUserActivity.Query().Get().Where(x => x.Uid == Uid).OrderByDescending(x => x.ActivityID).FirstOrDefault();

            return objUserActivity;
        }
        public double GetUserActivityByUidDateRange(int Uid, DateTime? dateFrom, DateTime? dateTo)
        {
            var objUserActivity = repoUserActivity.Query()
                                    .Filter(x => x.Uid == Uid && x.UserLogin.IsActive == true && x.Date >= dateFrom && x.Date <= dateTo)
                                    .OrderBy(o => o.OrderByDescending(x => x.ActivityID))
                                    .GetQuerable().GroupBy(x => x.Date).Distinct().Count();

            return objUserActivity;
        }

        public List<UserLiteActivityLogDto> GetUserActivityLog(out int total, PagingService<UserActivityLog> pagingService, bool descOrder = true)
        {

            var results = repoUserActivityLog.Query()
                .Filter(pagingService.Filter).GetQuerable().GroupBy(u => new { u.Date, u.Uid });

            var result = new List<UserLiteActivityLogDto>();
            if (descOrder)
            {
                result = results.OrderByDescending(u => u.Key.Uid)
                         .Skip(pagingService.Start - 1).Take(pagingService.Length)
                         .Select(u => new UserLiteActivityLogDto
                         {
                             Uid = u.Key.Uid,
                             Date = u.Key.Date,
                             Projects = u.OrderBy(s => s.DateAdded)
                             .Select(s => new UserActivityLogProjectDto
                             {
                                 ProjectId = s.ProjectId,
                                 ProjectName = s.ProjectName,
                                 Status = (s.ProjectId.HasValue && s.Project.CRMProjectId == 0)
                                 ? "Free" : s.Status,
                                 DateAdded = s.DateAdded
                             }).ToList()
                         }).Distinct()
                         .ToList();
            }
            else
            {
                result = results.OrderBy(u => u.Key.Uid)
                .Skip(pagingService.Start - 1).Take(pagingService.Length).Select(u => new UserLiteActivityLogDto { Uid = u.Key.Uid, Date = u.Key.Date, Projects = u.OrderBy(s => s.DateAdded).Select(s => new UserActivityLogProjectDto { ProjectId = s.ProjectId, ProjectName = s.ProjectName, Status = (s.ProjectId.HasValue && s.Project.CRMProjectId == 0) ? "Free" : s.Status, DateAdded = s.DateAdded }).ToList() }).Distinct().ToList();
            }
            total = repoUserActivityLog.Query()
                .Filter(pagingService.Filter).GetQuerable().GroupBy(u => new { u.Date, u.Uid }).Count();
            return result;
        }

        public List<ActivityFreeReportDto> GetUserActivityLog(out int total, int noOfFreeDays, PagingService<UserActivityLog> pagingService)
        {
            //var TeamManagers = repoUserMaster.Query().Filter(u => u.PMUid == 0 || u.PMUid == null || !u.PMUid.HasValue).Get().ToList();

            var TeamManagers = repoUserMaster.Query().Filter(R => (R.RoleId == (int)Enums.UserRoles.PMO || R.RoleId == (int)Enums.UserRoles.PM || R.RoleId == (int)Enums.UserRoles.Director
          || RoleValidator.HR_RoleIds.Contains(R.RoleId.Value)) && (R.IsActive == true || R.IsActive == false)).Get().OrderBy(T => T.UserName).ToList();

            var filteredQuery = repoUserActivityLog.Query()
                            .Filter(pagingService.Filter)
                            .Include(x => x.UserLogin)
                            .GetQuerable()
                            .GroupBy(u => u.Uid);

            if (noOfFreeDays > 0)
            {
                filteredQuery = filteredQuery.Where(x => x.Select(d => d.Date).Distinct().Count() >= noOfFreeDays);
            }

            var results = filteredQuery.ToList();
            total = results.Count;

            return results.Select(x =>
            {
                var activity = x.FirstOrDefault();
                return new ActivityFreeReportDto
                {
                    UserId = activity.Uid.Value,
                    Name = activity.UserLogin.Name,
                    Department = activity.UserLogin.Department.Name,
                    Designation = activity.UserLogin.Role?.RoleName,
                    TeamManager = TeamManagers.FirstOrDefault(m => m.Uid == (activity.UserLogin.PMUid ?? 0))?.Name,
                    TotalFreeDays = x.Select(d => d.Date).Distinct().Count()
                };
            }).OrderByDescending(x => x.TotalFreeDays).ThenBy(x => x.Name).ToList();

        }

        public ActivityFreeReportDto GetUserFreeActivityDetails(int uid, DateTime startDate, DateTime endDate, List<DateTime> excludingDates)
        {
            var activities = repoUserActivityLog.Query()
                            .Filter(x => x.Uid == uid && x.Date >= startDate && x.Date <= endDate && !excludingDates.Contains(x.Date) && x.Status.ToLower() == "free")
                            .OrderBy(o => o.OrderByDescending(x => x.Date))
                            .Get().ToList();

            ActivityFreeReportDto details = null;

            if (activities.Any())
            {
                var user = activities.FirstOrDefault().UserLogin;

                details = new ActivityFreeReportDto
                {
                    FreeDetails = activities.GroupBy(x => x.Date).Select(x =>
                     new ActivityFreeDetaiDto
                     {
                         Date = x.Key.ToFormatDateString("ddd, MMM dd yyyy"),
                         Description = string.Join("<br>", x.Select(a => a.ProjectName)),
                         TimeSheet = string.Join("<br>", user.UserTimeSheets1.Where(ut => ut.AddDate == x.Key.Date)
                                                .Select(ut => string.Format("* <b>{0}</b> : ({1:hh\\:mm}) {2}", ut.Project?.Name, ut.WorkHours, ut.Description)))
                     }).ToList()
                };

                details.UserId = user.Uid;
                details.Designation = user.Role?.RoleName;
                details.Department = user.Department.Name;
                details.Name = user.Name;
                details.TotalFreeDays = activities.Select(d => d.Date).Distinct().Count();
            }

            return details;
        }

        public void GetTotalUserActivityLog(out int totalpaid, out int totalfree, PagingService<UserActivityLog> pagingService)
        {

            var results = repoUserActivityLog.Query()
                .Filter(pagingService.Filter).GetQuerable().GroupBy(u => new { u.Date, u.Uid });
            var days = results.Select(u => u.Select(s => new { DateAdded = s.DateAdded, ProjectId = s.ProjectId, CRMProjectId = s.ProjectId.HasValue ? s.Project.CRMProjectId : 0 })).ToList();
            totalpaid = 0;
            totalfree = 0;
            foreach (var item in days)
            {
                if (item.Count() == 1)
                {
                    if (item.FirstOrDefault().ProjectId.HasValue && item.FirstOrDefault().CRMProjectId != 0)
                    {
                        totalpaid++;
                    }
                    else
                    {
                        totalfree++;
                    }
                }
                else
                {
                    bool isLastWorking = item.OrderByDescending(s => s.DateAdded).FirstOrDefault().ProjectId.HasValue && item.OrderByDescending(s => s.DateAdded).FirstOrDefault().CRMProjectId != 0;
                    double totalFree = 0, totalPaid = 0;
                    foreach (var sitem in item.Skip(0).Take(item.Count() - 1).Select((proj, i) => new { proj, i }))
                    {
                        TimeSpan daydiff = (item.Skip(sitem.i + 1).Take(1).FirstOrDefault().DateAdded - sitem.proj.DateAdded);
                        if (sitem.proj.ProjectId.HasValue && sitem.proj.CRMProjectId != 0)
                        {
                            totalPaid = totalPaid + daydiff.TotalHours;
                        }
                        else
                        {
                            totalFree = totalFree + daydiff.TotalHours;
                        }
                    }
                    if (isLastWorking)
                    {
                        if ((totalFree / 60) >= 4)
                        {
                            totalfree++;
                        }
                        else
                        {
                            totalpaid++;
                        }
                        //item.Select((s,i) => new { });
                    }
                    else
                    {
                        if ((totalPaid / 60) >= 4)
                        {
                            totalpaid++;
                        }
                        else
                        {
                            totalfree++;
                        }
                    }
                }
            }
            //totalpaid = days.Sum(a => a.ProjectId.HasValue ? 1 : 0);
            //totalfree = days.Sum(a => a.ProjectId.HasValue ? 0 : 1);
        }

        public void Delete(UserActivity entity)
        {
            if (entity != null)
            {
                repoUserActivity.Delete(entity.ActivityID);
                repoUserActivity.SaveChanges();
            }

        }

        public void Save(UserActivity entity)
        {
            if (entity.ActivityID == 0)
            {
                if (entity.UserActivityManageProject != null && entity.UserActivityManageProject.Count > 0)
                {
                    repoUserActivity.ChangeEntityCollectionState<UserActivityManageProject>(entity.UserActivityManageProject, ObjectState.Added);
                }
                repoUserActivity.ChangeEntityState<UserActivity>(entity, ObjectState.Added);
            }
            else
            {
                repoUserActivity.ChangeEntityState<UserActivity>(entity, ObjectState.Modified);
            }
            repoUserActivity.SaveChanges();
        }

        public List<UserActivity> GetUserActivityForResourcePoolByDepartment(int pmuid, int? deptId = null)
        {
            //To get the developer lists
            DateTime date = DateTime.Now.Date;

            IEnumerable<UserActivity> lsAllUsers = repoUserActivity.Query().Filter(u => u.UserLogin.IsActive == true && (u.Date.HasValue ? u.Date.Value == date : false)).Get().ToList();

            IEnumerable<UserActivity> lsUsers = lsAllUsers.AsEnumerable()
                                                        .Where(U => U.Date.CompareDate(DateTime.Now) && U.UserLogin.PMUid == pmuid &&
                                                                   U.UserLogin.IsActive == true &&
                                                                   (deptId.HasValue && deptId > 0 ? (U.UserLogin.DeptId == deptId) : true) &&
                                                                   U.UserLogin.LeaveActivities1.Count(L => (DateTime.Today >= L.StartDate && DateTime.Today <= L.EndDate) && (L.Status == (int)Enums.LeaveStatus.Approved || L.Status == (int)Enums.LeaveStatus.UnApproved)) == 0 &&
                                                                    !(U.UserLogin.RoleId == (int)Enums.UserRoles.HRBP || U.UserLogin.RoleId == (int)Enums.UserRoles.PM || RoleValidator.BA_RoleIds.Contains(U.UserLogin.RoleId.Value) || U.UserLogin.RoleId == (int)Enums.UserRoles.QA || U.UserLogin.RoleId == (int)Enums.UserRoles.UIUXDesigner || U.UserLogin.RoleId == (int)Enums.UserRoles.UIUXDeveloper || U.UserLogin.RoleId == (int)Enums.UserRoles.UIUXFrontEndDeveloper || U.UserLogin.RoleId == (int)Enums.UserRoles.UIUXManagerial || U.UserLogin.RoleId == (int)Enums.UserRoles.UIUXMeanStackDeveloper)
                                                                    && !(U.UserLogin.DeptId == (int)Enums.ProjectDepartment.QualityAnalyst || U.UserLogin.DeptId == (int)Enums.ProjectDepartment.WebDesigning))

                                                        .GroupBy(U => U.Uid)
                                                        .Select(G =>
                                                        {
                                                            UserActivity obj = G.FirstOrDefault(U => U.ActivityID == G.Max(A => A.ActivityID));
                                                            return obj;
                                                        });

            
            IEnumerable<UserActivity> lsNotLoggedUsers = repoUserMaster.Query()
                                                            .Filter(U => U.PMUid == pmuid &&
                                                                        U.IsActive == true &&
                                                                       (deptId.HasValue && deptId > 0 ? (U.DeptId == deptId) : true) &&
                                                                        (!(U.RoleId == (int)Enums.UserRoles.HRBP || U.RoleId == (int)Enums.UserRoles.PM || RoleValidator.BA_RoleIds.Contains(U.RoleId.Value) || RoleValidator.QA_RoleIds.Contains(U.RoleId.Value) || RoleValidator.UIUX_RoleIds.Contains(U.RoleId.Value)))
                                                                        && !(U.DeptId == (int)Enums.ProjectDepartment.BusinessAnalyst || U.DeptId == (int)Enums.ProjectDepartment.QualityAnalyst || U.DeptId == (int)Enums.ProjectDepartment.WebDesigning))
                                                                        .Get().Where(U => lsUsers.Select(A => A.Uid).Contains(U.Uid) == false)
                                                                        .Select(U =>
                                                                        {
                                                                            UserActivity obj = new UserActivity();
                                                                            obj.Uid = U.Uid;
                                                                            obj.Status = U.LeaveActivities1.Where(L => (DateTime.Today >= L.StartDate && DateTime.Today <= L.EndDate) && (L.Status == (int)Enums.LeaveStatus.Approved || L.Status == (int)Enums.LeaveStatus.UnApproved)).Count() > 0 ? (U.LeaveActivities1.Where(L => (DateTime.Today >= L.StartDate && DateTime.Today <= L.EndDate) && (L.Status == (int)Enums.LeaveStatus.Approved || L.Status == (int)Enums.LeaveStatus.UnApproved) && L.IsHalf == true).Count() > 0 ? "Leave-[Half]" : "Leave") : "Not Logged-In";
                                                                            obj.UserLogin = U;
                                                                            return obj;

                                                                        });

            return lsUsers.Concat(lsNotLoggedUsers).ToList();
        }

        public List<BucketModel> GetBucketModelsForResourcePool()
        {
            return repoBucketModel.Query().Filter(b => b.BucketId == 1 || b.BucketId == 2 || b.BucketId == 5).Get().ToList();
        }
        public class TechInfo
        {
            public string TechName { get; set; }
            public string SpecName { get; set; }

        }
        public List<ActivityGrid> GetActivities(DateTime date, int exceptUserID, int PMUid)
        {
            var TeamManagers = repoUserMaster.Query().Filter(u => u.PMUid == 0 || u.PMUid == null || !u.PMUid.HasValue).Get().ToList();
            var supportTeam = Common.GetSupportTeamRoleIds();
            var excludedBucketModels = Common.GetExcludedBucketModelIds();

            var allUsers = repoUserMaster.Query()
                .Filter(U => U.IsActive == true && U.PMUid != 0 && U.Uid != exceptUserID
                && (PMUid == 0 || U.PMUid == PMUid)
                && !(U.RoleId == (int)Enums.UserRoles.PM ||
                     U.RoleId == (int)Enums.UserRoles.Director || U.RoleId == (int)Enums.UserRoles.PMO ||
                     U.DeptId == (int)Enums.ProjectDepartment.AccountDepartment))
                //.Include(c => c.Department).Include(c => c.LeaveActivities1).Include(c => c.UserActivities).Include(c => c.ProjectDevelopers)
                .Get().ToList();
            Dictionary<string, int> enumSpecilityType = ((Enums.TechnologySpecializationType[])Enum.GetValues(typeof(Enums.TechnologySpecializationType))).ToDictionary(k => k.ToString(), v => (int)v);
            //var enumSpecilityType = Enum.GetValues(typeof(Enums.TechnologySpecializationType));
            //List<int> enumSpecilityTypeIds = ((Enums.TechnologySpecializationType[])Enum.GetValues(typeof(Enums.TechnologySpecializationType))).Select(c => (int)c).ToList();
            var retActivity = allUsers.Select(U =>
            {
                ActivityGrid usrAct = new ActivityGrid();
                usrAct.UserID = U.Uid;
                usrAct.Name = U.Name;
                usrAct.PmUID = U.PMUid;
                usrAct.UserDesignation = U.Role?.RoleName;
                usrAct.PmRole = U.PMUid.HasValue ? TeamManagers.FirstOrDefault(m => m.Uid == U.PMUid.Value)?.RoleId : 0;
                usrAct.TeamManager = U.PMUid.HasValue ? TeamManagers.FirstOrDefault(m => m.Uid == U.PMUid.Value)?.Name : "";
                usrAct.UserDepartmentID = U.DeptId;
                usrAct.RoleId = U.RoleId ?? 0;
                usrAct.UserDepartmentName = U.Department?.Name;
                usrAct.UserDepartment = GetUserDepartment(U.DeptId ?? 0, U.RoleId ?? 0, 0);
                usrAct.IsResigned = U.IsResigned;

                byte[] specilityTypeIds = U.User_Tech != null && U.User_Tech.Count > 0 ? U.User_Tech.Where(x => x.SpecTypeId.HasValue).Select(T => T.SpecTypeId.Value).ToArray() : new byte[] { };
                usrAct.Specialisties = specilityTypeIds.Select(x => ((Enums.TechnologySpecializationType)x).GetDescription()).ToArray();

                usrAct.Technologies = U.User_Tech != null ? U.User_Tech.Select(T => T.Technology.Title).ToArray() : new string[] { };

                usrAct.TechnologiesWithSpecility = U.User_Tech != null ? U.User_Tech.Where(x => x.SpecTypeId.HasValue).Select(i => new { i.Technology.Title, i.SpecTypeId }).ToList()
                                                              .Select(x => new KeyValuePair<string, string>(x.Title, ((Enums.TechnologySpecializationType)x.SpecTypeId).GetDescription())).ToList() :
                                                              new List<KeyValuePair<string, string>>();



                var userTechSpec = U.User_Tech != null ? U.User_Tech.Select(x => new KeyValuePair<string, int>(x.Technology.Title, x.SpecTypeId != null ? x.SpecTypeId.Value : 0)).ToList() : new List<KeyValuePair<string, int>>();
                var selectTextSpecWithTech = userTechSpec.Select(x => new KeyValuePair<string, string>(x.Key, x.Value != 0 ? ((Enums.TechnologySpecializationType)x.Value).GetDescription() : "")).ToList();
                List<TechInfo> TechInfoList = new List<TechInfo>();
                List<string> expertExcelTechInfoList = new List<string>();
                List<string> beginerExcelTechInfoList = new List<string>();
                List<string> intermediateExcelTechInfoList = new List<string>();
                List<string> intrestedExcelTechInfoList = new List<string>();

                foreach (KeyValuePair<string, string> item in selectTextSpecWithTech)
                {
                    var spanclass = string.Empty;
                    switch (item.Value)
                    {
                        case "Expert":
                            spanclass = "success";
                            usrAct.ExpertExcelTechSpecilization.Add(item.Key);
                            break;
                        case "Intermediate":
                            spanclass = "primary";
                            usrAct.InterExcelTechSpecilization.Add(item.Key);
                            break;
                        case "Beginner":
                            spanclass = "warning";
                            usrAct.BeignExcelTechSpecilization.Add(item.Key);
                            break;
                        case "Interested":
                            spanclass = "dark";
                            usrAct.InteresExcelTechSpecilization.Add(item.Key);
                            break;
                    }

                    TechInfoList.Add(new TechInfo() { TechName = item.Key, SpecName = item.Value.HasValue() ? "<span title=" + item.Value + " class='round-b badge-" + spanclass + "'></span>" : string.Empty });
                    spanclass = string.Empty;
                }


                usrAct.OtherTechnology = !string.IsNullOrEmpty(U.OtherTechnology) ? U.OtherTechnology : string.Empty;
                usrAct.TechSpecilization = TechInfoList.Select(x => x.SpecName + " " + x.TechName).ToArray();

                if (usrAct.OtherTechnology.HasValue())
                {
                    usrAct.TechSpecilization = usrAct.TechSpecilization.Concat(new string[] { "</br><strong> Other Technology :</strong>" + usrAct.OtherTechnology }).ToArray();
                }

                usrAct.DomainExperts = U.DomainExperts != null ? U.DomainExperts.Select(x => Convert.ToString(x.DomainId)).ToArray() : new string[] { };
                usrAct.DomainExpertName = repoDomainType.Query().Filter(x => usrAct.DomainExperts.Contains(Convert.ToString(x.DomainId))).GetQuerable().Select(x => x.DomainName).ToArray();
                usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.NotLoggedIn;
                usrAct.Status = "Not Logged-In";

                var projectDevelopers = U.ProjectDevelopers.Where(pd => pd.Project.PMUid == U.PMUid && pd.Project.Status == "R" &&
                                            pd.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running &&
                                            !pd.Project.IsInHouse &&
                                            pd.Project.Model.HasValue && !excludedBucketModels.Contains(pd.Project.Model ?? 0));

                usrAct.NotLogInButRunning = projectDevelopers.Count(pd => (pd.Project.Model ?? 0) != (int)Enums.BucketModel.SEO);

                if (U.LeaveActivities1.Any())
                {
                    LeaveActivity leave = U.LeaveActivities1.FirstOrDefault(L => DateTime.Today.Date >= L.StartDate.Date &&
                                                DateTime.Today.Date <= L.EndDate.Date &&
                                                L.Status.HasValue && L.Status.Value != (int)Enums.LeaveStatus.Cancelled);
                    if (leave != null)
                    {
                        if (leave.Status == (int)Enums.LeaveStatus.Approved)
                        {
                            if (leave.IsHalf.HasValue && leave.IsHalf.Value)
                            {
                                usrAct.Status = "Leave-[Half]";
                                usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.LeaveHalf;
                            }
                            else
                            {
                                usrAct.Status = "Leave";
                                usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.Leave;
                            }

                            // Assign to 'OnLeaveButRunning' and Reset 'NotLogInButRunning' because person is on leave
                            usrAct.OnLeaveButRunning = usrAct.NotLogInButRunning;
                            usrAct.NotLogInButRunning = 0;
                        }
                        else if (leave.Status == (int)Enums.LeaveStatus.UnApproved)
                        {
                            usrAct.Status = "Not Logged-In [UnApproved]";
                            usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.NotLoggedInLeaveUnApproved;
                        }
                        else
                        {
                            usrAct.Status = "Not Logged-In [Leave Pending]";
                            usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.NotLoggedInLeavePending;
                        }
                    }
                }

                usrAct.AvailabilityStatus = usrAct.Status;

                usrAct.LoginTime = "-";
                usrAct.CRM_ProjectStatus = "-";

                if (U.UserActivities.Any())
                {
                    var activityList = U.UserActivities.Where(a => a.Date.CompareDate(date));
                    var recentActivity = activityList.OrderByDescending(a => a.DateAdded).FirstOrDefault();
                    if (recentActivity != null)
                    {
                        usrAct.ActivityID = recentActivity.ActivityID;
                        usrAct.ProjectName = recentActivity.UserActivityManageProject.Count() == 0 ? recentActivity.ProjectName : string.Empty;

                        usrAct.ProjectModel = recentActivity.UserActivityManageProject.Count() == 0 ? recentActivity.Project?.BucketModel?.ModelName : string.Join("<br />", recentActivity.UserActivityManageProject.OrderByDescending(x => x.ProjectStatus).Select(x => $"<span class='round-b badge-{(Common.GetProjectDisplayStatus(x.ProjectStatus) == "Running" ? "success" : "warning")}' style='width:6px;height: 7px;'></span> {x.Project.Name} [{x.Project.CRMProjectId}] - {Common.GetProjectDisplayStatus(x.ProjectStatus)}{(Common.GetProjectDisplayStatus(x.ProjectStatus) == "Running" ? $" ({x.Project.ProjectDevelopers.Count(p => p.WorkStatus == (int)Core.Enums.ProjectDevWorkStatus.Running)})" : string.Empty)}"));

                        usrAct.ProjectID = recentActivity.ProjectId;
                        usrAct.Status = recentActivity.Status;
                        usrAct.Comment = recentActivity.Comment;

                        usrAct.RunningProjects = recentActivity.UserActivityManageProject.Count(x => x.ProjectStatus == "R");
                        usrAct.RunningDevelopers = recentActivity.UserActivityManageProject.Sum(x => x.Project.ProjectDevelopers.Count(c => c.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running));
                    }

                    var firstActivity = activityList.OrderBy(O => O.DateAdded).FirstOrDefault();
                    if (firstActivity != null)
                    {
                        usrAct.LoginTime = "<b>" + firstActivity.DateAdded.Value.ToShortTimeString() + "</b>";
                    }
                }

                var userActivity = U.UserActivities.OrderByDescending(x => x.DateAdded).FirstOrDefault(x => x.Date.Value.Date == DateTime.Today.Date);

                if (userActivity != null && userActivity.ProjectId.HasValue)
                {
                    usrAct.NotLogInButRunning = 0; // Reset because user is logged in
                    usrAct.CRM_ProjectStatus = Common.GetProjectDisplayStatus(userActivity.Project.Status);

                    ProjectAdditionalSupport addSupport = null;
                    if (!userActivity.Project.IsInHouse)
                    {
                        addSupport = userActivity.Project.ProjectAdditionalSupports
                                        .Where(x => x.ProjectAdditionalSupportUser.Any(pu => pu.AssignedUid == usrAct.UserID) && x.StartDate.Date <= DateTime.Today && x.EndDate.Date >= DateTime.Today)
                                        .OrderByDescending(x => x.CreateDate).FirstOrDefault();
                    }

                    if (userActivity.Project.Status == "R")
                    {
                        if (userActivity.Project.ProjectDevelopers.Any(x => x.Uid == userActivity.Uid && x.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running))
                        {
                            if (userActivity.Project.IsInHouse)
                            {
                                usrAct.AvailabilityStatus = "In-House";
                                usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.InHouse;
                            }
                            else
                            {
                                usrAct.AvailabilityStatus = "Running";
                                usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.Running;

                                usrAct.IsBucketProject = userActivity.Project.Model.HasValue && excludedBucketModels.Any(b => b == userActivity.Project.Model);
                                usrAct.IsSEOProject = userActivity.Project.Model.HasValue && userActivity.Project.Model.Value == (int)Enums.BucketModel.SEO;
                            }
                            usrAct.OnLeaveButRunning = 0;  // Reset the count if any user which are on leave but select any his/her running project
                        }
                        else
                        {
                            if (userActivity.Project.IsInHouse)
                            {
                                usrAct.AvailabilityStatus = "In-House";
                                usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.InHouse;
                            }
                            else
                            {
                                usrAct.AvailabilityStatus = "Additional Support";
                                usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.AdditionalSupport;

                                if (addSupport != null)
                                {
                                    usrAct.AdditionalSupportPeriod = $"{addSupport.StartDate.ToFormatDateString("dd/MM/yy")} to {addSupport.EndDate.ToFormatDateString("dd/MM/yy")}";
                                    usrAct.AdditionalSupportReason = addSupport.Description;
                                    usrAct.AdditionalSupportStatus = (Enums.AddSupportRequestStatus)addSupport.Status;
                                }
                                else if (supportTeam.Contains(U.RoleId ?? 0))
                                {
                                    usrAct.AvailabilityStatus = "Support Team";
                                    usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.SupportTeam;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (userActivity.Project.IsInHouse)
                        {
                            usrAct.AvailabilityStatus = "In-House";
                            usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.InHouse;
                        }
                        else if (addSupport != null)
                        {
                            usrAct.AvailabilityStatus = "Additional Support";
                            usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.AdditionalSupport;

                            usrAct.AdditionalSupportPeriod = $"{addSupport.StartDate.ToFormatDateString("dd/MM/yy")} to {addSupport.EndDate.ToFormatDateString("dd/MM/yy")}";
                            usrAct.AdditionalSupportReason = addSupport.Description;
                            usrAct.AdditionalSupportStatus = (Enums.AddSupportRequestStatus)addSupport.Status;
                        }
                        else
                        {
                            usrAct.AvailabilityStatus = "Working-Overrun";
                            usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.WorkingOverrun;
                        }
                    }
                }
                else if (userActivity != null)
                {
                    usrAct.NotLogInButRunning = 0; // Reset because user is logged in

                    if (supportTeam.Contains(U.RoleId ?? 0) || usrAct.Status == "Support Team")
                    {
                        usrAct.AvailabilityStatus = "Support Team";
                        usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.SupportTeam;
                    }
                    else
                    {
                        usrAct.AvailabilityStatus = "Free";
                        usrAct.AvailabilityStatusOrder = (int)Enums.ActivityStatusOrder.Free;
                    }
                }

                if (projectDevelopers.Any())
                {
                    if (usrAct.OnLeaveButRunning == 0 && usrAct.NotLogInButRunning == 0)
                    {
                        usrAct.AssignedAddon = projectDevelopers.Count(pd =>
                                    (userActivity == null || userActivity.ProjectId == null || pd.ProjectId != userActivity.ProjectId) &&
                                    (pd.Project.Model ?? 0) != (int)Enums.BucketModel.SEO);
                    }

                    usrAct.SEOProjectRunningDev = projectDevelopers.Count(pd => (pd.Project.Model ?? 0) == (int)Enums.BucketModel.SEO);
                }

                return usrAct;
            });

            return retActivity.ToList();
        }
        public List<ActivityGrid> GetActivitiesnew(DateTime date, int exceptUserID, int PMUid)
        {
            var connectionString = configuration["ConnectionStrings:dsmanagementConnection"];
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(connectionString);
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
            System.Data.DataSet ds = new System.Data.DataSet();
            cmd = new System.Data.SqlClient.SqlCommand("GetActivityData", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PMUid", PMUid);//if you have parameters.
            cmd.CommandTimeout = 500;
            da = new System.Data.SqlClient.SqlDataAdapter(cmd);
            da.Fill(ds);
            con.Close();
            List<ActivityGrid> retActivity = new List<ActivityGrid>();
            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            {
                int _designationId = !string.IsNullOrWhiteSpace(dr["DesignationId"].ToString()) ? Convert.ToInt32(dr["DesignationId"]) : 0;
                string status = dr["CRM_ProjectStatus"].ToString() == "Hold" ? dr["Status"].ToString() == "Working-Overrun" ? dr["Status"].ToString() : "Additional Support" : dr["Status"].ToString();
                Core.Enums.ProjectDepartment UserDepartment = GetUserDepartment(Convert.ToInt32(dr["UserDepartmentID"]), Convert.ToInt32(dr["RoleId"]), _designationId);
                
                string Technology = dr["Technologies"].ToString();
                List<string> list = new List<string>();
                foreach (var tech in Technology.Split(',').ToList())
                {
                    if (!string.IsNullOrEmpty(tech))
                    {
                        list.Add(tech);
                    }
                }
                string domainExperts = dr["DomainExperts"].ToString();
                List<string> dlist = new List<string>();
                foreach (var dmn in domainExperts.Split(',').ToList())
                {
                    if (!string.IsNullOrEmpty(dmn))
                    {
                        dlist.Add(dmn);
                    }
                }
                Core.Enums.AddSupportRequestStatus? AdditionalSupport_Status = null;
                if (dr["AdditionalSupportStatus"].ToString() != "")
                {
                    AdditionalSupport_Status = (Enums.AddSupportRequestStatus)Convert.ToInt32(dr["AdditionalSupportStatus"]);
                }
                //int BucketRunning = Convert.ToInt32(dr["IsBucketProject"]);
                retActivity.Add(new ActivityGrid
                {
                    UserID = Convert.ToInt32(dr["UserID"]),
                    Name = dr["Name"].ToString(),
                    UserDesignation = dr["UserDesignation"].ToString(),
                    TeamManager = dr["TeamManager"].ToString(),
                    PmUID = Convert.ToInt32(dr["PmUID"]),
                    RoleId = Convert.ToInt32(dr["RoleId"]),
                    UserDepartmentName = dr["UserDepartment"].ToString(),
                    UserDepartment = UserDepartment,
                    ProjectName = dr["ProjectName"].ToString(),
                    ProjectModel = dr["ProjectModel"].ToString(),
                    AvailabilityStatus = status,
                    TechSpecilization = new string[] { dr["TechSpecilization"].ToString() },
                    Technologies = list.ToArray(),
                    DomainExpertName = new string[] { dr["DomainExpertName"].ToString() },
                    DomainExperts = dlist.ToArray(),
                    CRM_ProjectStatus = dr["CRM_ProjectStatus"].ToString(),
                    LoginTime = dr["LoginTime"].ToString(),
                    IsResigned = Convert.ToBoolean(dr["IsResigned"]),
                    PmRole = Convert.ToInt32(dr["PmRole"]),
                    UserDepartmentID = Convert.ToInt32(dr["UserDepartmentID"]),
                    AvailabilityStatusOrder = status == "Free" ? (int)Enums.ActivityStatusOrder.Free : status == "Additional Support" ? (int)Enums.ActivityStatusOrder.AdditionalSupport : status == "Support Team" ? (int)Enums.ActivityStatusOrder.SupportTeam : status == "Leave" ? (int)Enums.ActivityStatusOrder.Leave : status == "Running" ? (int)Enums.ActivityStatusOrder.Running : status == "Not Logged-In" ? (int)Enums.ActivityStatusOrder.NotLoggedIn : status == "In-House" ? (int)Enums.ActivityStatusOrder.InHouse : status == "Leave-[Half]" ? (int)Enums.ActivityStatusOrder.LeaveHalf : status == "Working-Overrun" ? (int)Enums.ActivityStatusOrder.WorkingOverrun : 100,
                    IsBucketProject = Convert.ToBoolean(dr["IsBucketProject"]),
                    IsSEOProject = Convert.ToBoolean(dr["IsSEOProject"]),
                    ProjectID = Convert.ToInt32(dr["ProjectId"]),
                    AssignedAddon = Convert.ToInt32(dr["AssignedAddon"]),
                    SEOProjectRunningDev = Convert.ToInt32(dr["SEOProjectRunningDev"]),
                    OnLeaveButRunning = Convert.ToInt32(dr["OnLeaveButRunning"]),
                    NotLogInButRunning = Convert.ToInt32(dr["NotLogInButRunning"]),
                    ExpertExcelTechSpecilization = new List<string> { dr["ExpertExcelTechSpecilization"].ToString() },
                    InterExcelTechSpecilization = new List<string> { dr["InterExcelTechSpecilization"].ToString() },
                    BeignExcelTechSpecilization = new List<string> { dr["BeignExcelTechSpecilization"].ToString() },
                    InteresExcelTechSpecilization = new List<string> { dr["InteresExcelTechSpecilization"].ToString() },
                    OtherTechnology = dr["OtherTechnology"].ToString(),
                    RunningProjects = dr["RunningProjects"].ToString() != "" ? Convert.ToInt32(dr["RunningProjects"]) : 0,
                    RunningDevelopers = dr["RunningDevelopers"].ToString() != "" ? Convert.ToInt32(dr["RunningDevelopers"]) : 0,
                    AdditionalSupportPeriod = dr["AdditionalSupportPeriod"].ToString(),
                    AdditionalSupportStatus = AdditionalSupport_Status,
                    DesignationId = _designationId
                    //isPMUser= dr["productID"].ToString()
                });
            }

            return retActivity;
        }

        public List<UserActivity> GetUserActivitiesdataBetweenTwoDate(DateTime fromDate, DateTime toDate)
        {
            return repoUserActivity.Query().Filter(D => D.Date >= fromDate.Date && D.Date <= toDate.Date).Get().ToList();
        }

        public void DeleteUserActivityOfLastDay()
        {
            DateTime dtNow = DateTime.Now.AddDays(-15).Date;
            List<UserActivity> userActivity = repoUserActivity.Query().Filter(D => D.DateAdded != null && D.DateAdded.Value <= dtNow).Get().ToList();

            if (userActivity.Any())
            {
                repoUserActivity.DeleteBulk(userActivity);
                repoUserActivity.SaveChanges();
            }
        }

        private Enums.ProjectDepartment GetUserDepartment(int departmentId, int roleId, int designationId)
        {
            //if (roleId == (int)Enums.UserRoles.ST)
            //{
            //    return Enums.ProjectDepartment.SoftwareTrainee;
            //}
            if (RoleValidator.Trainee_DesignationIds.Contains(designationId))
            {
                return Enums.ProjectDepartment.SoftwareTrainee;
            }

            if (departmentId == 0)
            {
                return Enums.ProjectDepartment.Other;
            }

            var department = (Enums.ProjectDepartment)departmentId;
            switch (department)
            {
                case Enums.ProjectDepartment.AccountDepartment:
                case Enums.ProjectDepartment.DotNetDevelopment:
                case Enums.ProjectDepartment.PHPDevelopment:
                case Enums.ProjectDepartment.MobileApplication:
                case Enums.ProjectDepartment.HubSpot:
                case Enums.ProjectDepartment.BusinessAnalyst:
                case Enums.ProjectDepartment.QualityAnalyst:
                case Enums.ProjectDepartment.SEO:
                case Enums.ProjectDepartment.WebDesigning:
                case Enums.ProjectDepartment.SoftwareTrainee:
                    // do nothing
                    break;
                default:
                    department = Enums.ProjectDepartment.Other;
                    break;
            }

            return department;
        }

        public void GetTotalPaidDays(int uid, DateTime startDate, DateTime endDate, List<DateTime> excludingDates, out int paidDays)
        {
            paidDays = 0;
            paidDays = repoUserActivityLog.Query().Filter(al => al.Uid == uid
                                                                 && al.ProjectId != null
                                                                 && !al.Project.IsInHouse
                                                                 && al.Date >= startDate.Date
                                                                 && al.Date <= endDate.Date
                                                                 && !excludingDates.Contains(al.Date)).Get()
                                                                .Select(a => a.Date).Distinct().Count();
        }


        public List<ActivityFreeDetaiDto> GetUserActivityDetails(int uid, DateTime startDate, DateTime endDate, List<DateTime> excludingDates, Enums.ActivityDetail activityDetail)
        {
            List<UserActivityLog> activities = null;
            if (activityDetail == Enums.ActivityDetail.Free)
            {
                activities = repoUserActivityLog.Query()
                            .Filter(x => x.Uid == uid && x.Date >= startDate.Date
                            && x.Date <= endDate.Date
                            && !excludingDates.Contains(x.Date)
                            && x.Status.ToLower() == "free")
                            .OrderBy(o => o.OrderByDescending(x => x.Date))
                            .Get().ToList();
            }
            else if (activityDetail == Enums.ActivityDetail.Paid)
            {
                activities = repoUserActivityLog.Query()
                            .Filter(x => x.Uid == uid && x.Date >= startDate.Date
                            && x.Date <= endDate.Date
                            && !excludingDates.Contains(x.Date)
                            && x.ProjectId != null
                            && !x.Project.IsInHouse)
                            .OrderBy(o => o.OrderByDescending(x => x.Date))
                            .Get().ToList();
            }
            else
            {
                activities = repoUserActivityLog.Query()
                            .Filter(x => x.Uid == uid && x.Date >= startDate.Date && x.Date <= endDate.Date && !excludingDates.Contains(x.Date))
                            .OrderBy(o => o.OrderByDescending(x => x.Date))
                            .Get().ToList();
            }

            if (activities.Any())
            {
                var user = activities.FirstOrDefault().UserLogin;


                return activities.GroupBy(x => x.Date).Select(x =>
                  new ActivityFreeDetaiDto
                  {
                      Date = x.Key.ToFormatDateString("ddd, MMM dd yyyy"),
                      Description = string.Join("<br>", x.Select(a => a.ProjectName)),
                      TimeSheet = string.Join("<br>", user.UserTimeSheets1.Where(ut => ut.AddDate == x.Key.Date)
                                             .Select(ut => string.Format("* <b>{0}</b> : ({1:hh\\:mm}) {2}", ut.Project?.Name, ut.WorkHours, ut.Description)))
                  }).ToList();
            }

            return null;
        }

        public List<UserActivityLog> GetUserLoginActivityDetails(int month, int year)
        {
            var activities = repoUserActivityLog.Query()
                            .Filter(x => (x.Date.Month == month) && x.Date.Year == year)
                            .Get().ToList();

            return activities;
        }

        public List<UserActivityLog> GetActivitiesInDuration(int uid, DateTime? startDate, DateTime? endDate, List<DateTime> excludingDates)
        {
            var expr = PredicateBuilder.True<UserActivityLog>();
            expr = expr.And(al => al.Uid == uid && al.UserLogin.IsActive == true && !excludingDates.Contains(al.Date));
            if (startDate.HasValue && endDate.HasValue)
            {
                expr = expr.And(al => al.Date >= startDate.Value.Date && al.Date <= endDate.Value.Date);
            }
            else if (startDate.HasValue)
            {
                expr = expr.And(al => al.Date >= startDate.Value.Date);
            }
            else if (endDate.HasValue)
            {
                expr = expr.And(al => al.Date <= endDate.Value.Date);
            }

            return repoUserActivityLog.Query()
                .Filter(expr)
                .OrderBy(c => c.OrderByDescending(al => al.Date))
                .Get()
                .GroupBy(al => new { al.Uid, al.Date })
                .Select(G => new UserActivityLog { Date = G.Key.Date, Uid = G.Key.Uid, DateAdded = G.Min(al => al.DateAdded) })
                .ToList();
        }

        public List<UserActivityLog> GetUserLoginActivityDetails(DateTime? AttendanceDate)
        {
            var activities = repoUserActivityLog.Query()
                            .Filter(x => (x.Date == AttendanceDate))
                            .Get().ToList();

            return activities;
        }

        public List<TeamStatusReportGraph_Result> GetTeamStatusReportGraph(int uid, DateTime? startDate, DateTime? endDate)
        {
           return repoUserActivity.GetDbContext().GetAciveUserOnDate(uid, startDate, endDate).Result.ToList();
        }
        
        public List<AllTeamStatusReportGraph_Result> GetAllTeamStatusReportGraph(int uid, DateTime? startDate, DateTime? endDate)
        {
           return repoUserActivity.GetDbContext().GetAllTeamActiveUserOnDate(uid, startDate, endDate).Result.ToList();
        }

        public List<TeamStatusReportGraphDetail_Result> GetTeamStatusReportGraphDetails(int uid, DateTime? selectedDate)
        {
            return repoUserActivity.GetDbContext().GetAciveUserDetailOnDate(uid, selectedDate).Result.OrderBy(x => x.Employee).ToList();
        } 
        public List<TeamStatusReportGraphDetail_Result> GetAllTeamStatusReportGraphDetails(int uid, DateTime? selectedDate)
        {
            return repoUserActivity.GetDbContext().GetAllAciveUserDetailOnDate(uid, selectedDate).Result.OrderBy(x=>x.Employee).ToList();
        }

    }
}

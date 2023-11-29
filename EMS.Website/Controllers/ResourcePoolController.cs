using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMS.Data;
using EMS.Service;
using EMS.Web.Code.Attributes;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Web.Code.LIBS;
using EMS.Dto;
using System.Globalization;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Controllers
{
    //[CustomAuthorization()]
    public class ResourcePoolController : BaseController
    {
        #region "Fields"
        private IUserLoginService userLoginServices;
        private IUserActivityService userActivityService;
        private IDepartmentService departmentServices;
        #endregion

        #region "Constructor"
        public ResourcePoolController(IUserLoginService userLoginServices, IDepartmentService departmentServices,
            IUserActivityService userActivityService)
        {
            this.departmentServices = departmentServices;
            this.userLoginServices = userLoginServices;
            this.userActivityService = userActivityService;
        }
        #endregion


        // GET: ManageProject
        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult Index()
        {

            ViewBag.DepartmentList = departmentServices.GetActiveDepartments().OrderBy(x => x.Name).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.DeptId.ToString()
            }).ToList();

            ViewBag.PMList = userLoginServices.GetUserByRole(Convert.ToInt32(Enums.UserRoles.PM), true).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Uid.ToString() + "," + (x.CRMUserId != null ? x.CRMUserId.ToString() : "0")
            }).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, string pmId, int deptId)
        {
            List<DeveloperDto> FinalListDevelopers = GetResourceList(pmId, deptId);
            return DataTablesJsonResult(FinalListDevelopers.Count(), request, FinalListDevelopers.Select((r, index) => new
            {
                r.ProjectId,
                rowIndex = (index + 1) + (request.Start),
                r.DeveloperName,
                r.UsersListSameProject,
                r.ProjectName,
                r.ModelName,
                r.DepartmentName,
                r.Status
            }));
        }


        public ActionResult ExportToExcel(string pmId, int deptId)
        {
            List<DeveloperDto> FinalListDevelopers = GetResourceList(pmId, deptId);
            if (FinalListDevelopers.Any())
            {
                var data = FinalListDevelopers.Select(x => new
                {
                    Name = x.DeveloperName,
                    Projet = x.ProjectName,
                    BucketModel = x.ModelName,
                    Group = x.GroupName,
                    Department = x.DepartmentName,
                    Status = x.Status
                }).ToList();
                string Reportname = "Resourcepool";
                Int32 subsheet = 0;

                return DownloadExcelFile(data, Reportname, subsheet);
            }
            return RedirectToAction("Index", "ResourcePool", new { Area = "" });
        }
        private List<DeveloperDto> GetResourceList(string pmId, int deptId)
        {
            int pmuid = 0;
            int CrmUserid = 0;
            if (CurrentUser.RoleId == Convert.ToInt32(Enums.UserRoles.HRBP))
            {
                if (!string.IsNullOrEmpty(pmId) && pmId != "0")
                {
                    string[] idArr = pmId.Split(',');
                    pmuid = Convert.ToInt32(idArr[0]);
                    CrmUserid = Convert.ToInt32(idArr[1]);
                }
            }
            else
            {

                if (CurrentUser.RoleId != (Convert.ToInt32(Enums.UserRoles.PM)))
                {
                    UserLogin obj = userLoginServices.GetUserInfoByID(CurrentUser.PMUid);
                    if (obj != null)
                        CrmUserid = obj.CRMUserId != null ? obj.CRMUserId.Value : 0;
                }
                else
                {
                    CrmUserid = CurrentUser.CRMUserId;
                }
                pmuid = Convert.ToInt32(CurrentUser.RoleId == (int)Enums.UserRoles.PM
                  ? (Convert.ToInt32(CurrentUser.Uid)) : (Convert.ToInt32(CurrentUser.PMUid)));
            }



            var lst = userActivityService.GetUserActivityForResourcePoolByDepartment(pmuid, deptId);
            string ApiPass = "";
            UserLogin objUser = userLoginServices.GetUserInfoByID(pmuid);
            if (objUser != null)
            {
                ApiPass = String.IsNullOrEmpty(objUser.ApiPassword) ? "" : objUser.ApiPassword;
            }

            string jsonString = new WebClient().DownloadString(SiteKey.CrmProjectList + CrmUserid + "&apipass=" + ApiPass);

            //var jsonString = System.IO.File.ReadAllText("D:\\local\\EMS\\EMS.Web\\Crmdata.txt");
            jsonString = jsonString.Replace("\"virtual\":", "\"virtual_dev\":");
            jsonString = jsonString.Replace("\"dev_detail\":\"\"", "\"dev_detail\":[{}]");//\"virtual_dev\":{\"username\":\"\",\"email\":\"\"}
            JObject Jobj = JObject.Parse(jsonString);
            var token = (JArray)Jobj.SelectToken("response");
            String json = null;
            //string dbnotes = "";
            foreach (var item in token)
            {
                json = JsonConvert.SerializeObject(item.SelectToken("result"));
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();

            //if (json != null)
            //{
            //    jss.MaxJsonLength = Int32.MaxValue;
            //}


            List<DeveloperDto> FinalListDevelopers = new List<DeveloperDto>();

            var bucketList = userActivityService.GetBucketModelsForResourcePool();

            foreach (var item in lst)
            {
                if (item.UserLogin.RoleId != Convert.ToInt32(Enums.UserRoles.UIUXDesigner)|| item.UserLogin.RoleId != Convert.ToInt32(Enums.UserRoles.UIUXDeveloper) || item.UserLogin.RoleId != Convert.ToInt32(Enums.UserRoles.UIUXFrontEndDeveloper) || item.UserLogin.RoleId != Convert.ToInt32(Enums.UserRoles.UIUXManagerial) || item.UserLogin.RoleId != Convert.ToInt32(Enums.UserRoles.UIUXMeanStackDeveloper))
                {
                    int uid = item.Uid.Value;
                    int projectId = item.ProjectId ?? 0;
                    if (projectId == 0)
                    {
                        if (item.Status.Trim().ToLower() != Enums.ActivityStatus.Leave.ToString().Trim().ToLower())
                        {
                            var obj = new DeveloperDto
                            {
                                UserId = uid,
                                DeveloperName = item.UserLogin.Title + " " + item.UserLogin.Name,
                                DepartmentName = item.UserLogin.Department.Name,
                                Status = item.Status,
                                ProjectName = item.ProjectName ?? "N/A",
                                ModelName = "N/A"
                            };

                            FinalListDevelopers.Add(obj);
                            //FreeDevelopers.Add(obj);
                        }
                    }
                    else if (projectId > 0)
                    {
                        var obj = new DeveloperDto();
                        int crmId = item.Project.CRMProjectId;

                        if (json != null)
                        {
                            List<ResultDto> ProjectList = JsonConvert.DeserializeObject<List<ResultDto>>(json).ToList();
                            //List<Result> ProjectList = projects.response != null ? projects.response[0].result : null;
                            if (ProjectList != null)
                            {
                                var projectObj = ProjectList.FirstOrDefault(m => m.project_id == crmId);

                                if (projectObj != null)
                                {
                                    projectObj.totalDeveloper = projectObj.totalDeveloper > 0 ? projectObj.totalDeveloper : projectObj.developer;
                                    if (projectObj.developer > 0)
                                    {
                                        obj = new DeveloperDto
                                        {
                                            UserId = uid,
                                            DeveloperName = item.UserLogin.Title + " " + item.UserLogin.Name,
                                            DepartmentName = item.UserLogin.Department.Name,
                                            Status = item.Status,
                                            ProjectName = projectObj.project_name + " - " + projectId,
                                            ProjectId = item.ProjectId ?? 0
                                        };
                                        //var bucketList = UserLog.GetBucketModelsForResourcePool();
                                        if (projectObj.modelname.Trim().ToLower() != bucketList[0].ModelName.Trim().ToLower()
                                            && projectObj.modelname.Trim().ToLower() != bucketList[1].ModelName.Trim().ToLower()
                                            && projectObj.modelname.Trim().ToLower() != bucketList[2].ModelName.Trim().ToLower())
                                        {
                                            obj.ModelName = projectObj.modelname;
                                            obj.Status = item.Status;


                                            int ProjectDevCount = projectObj.totalDeveloper;
                                            var loggednDev = lst.Where(m => m.ProjectId == item.ProjectId).ToList();
                                            if (loggednDev.Count > ProjectDevCount)
                                            {
                                                string loggedinUsers = "";
                                                foreach (var dev in loggednDev)
                                                {
                                                    string devName = dev.UserLogin.Name;
                                                    loggedinUsers = loggedinUsers + devName + ", ";
                                                }
                                                if (!string.IsNullOrEmpty(loggedinUsers))
                                                    obj.UsersListSameProject = loggedinUsers.Substring(0, loggedinUsers.Length - 2);
                                            }


                                            FinalListDevelopers.Add(obj);
                                            //FreeDevelopers.Add(obj);
                                        }
                                        else
                                        {
                                            //RunningDevelopers.Add(obj);
                                        }

                                        ProjectList.FirstOrDefault(m => m.project_id == crmId).developer = projectObj.developer - 1;
                                    }
                                    else
                                    {
                                        obj = new DeveloperDto
                                        {
                                            UserId = uid,
                                            DeveloperName = item.UserLogin.Title + " " + item.UserLogin.Name,
                                            DepartmentName = item.UserLogin.Department.Name,
                                            Status = item.Status,
                                            ProjectName = projectObj.project_name + " - " + item.Project.CRMProjectId,
                                            ProjectId = item.ProjectId ?? 0,
                                            ModelName = projectObj.modelname
                                        };

                                        //FreeDevelopers.Add(obj);

                                        int ProjectDevCount = projectObj.totalDeveloper;
                                        var loggednDev = lst.Where(m => m.ProjectId == item.ProjectId).ToList();
                                        if (loggednDev.Count > ProjectDevCount)
                                        {
                                            string loggedinUsers = "";
                                            foreach (var dev in loggednDev)
                                            {
                                                string devName = dev.UserLogin.Name;
                                                loggedinUsers = loggedinUsers + devName + ", ";
                                            }
                                            if (!string.IsNullOrEmpty(loggedinUsers))
                                                obj.UsersListSameProject = loggedinUsers.Substring(0, loggedinUsers.Length - 2);
                                        }


                                        FinalListDevelopers.Add(obj);
                                    }
                                }
                                else
                                {
                                    obj = new DeveloperDto
                                    {
                                        UserId = uid,
                                        DeveloperName = item.UserLogin.Title + " " + item.UserLogin.Name,
                                        DepartmentName = item.UserLogin.Department.Name,
                                        Status = item.Status,
                                        ProjectName = item.Project != null ? item.Project.Name + " (Not Found)" : "Not Found",
                                        ModelName = "N/A",
                                    };
                                    FinalListDevelopers.Add(obj);
                                    //FreeDevelopers.Add(obj);
                                }

                            }
                        }
                        else
                        {
                            obj = new DeveloperDto
                            {
                                UserId = uid,
                                DeveloperName = item.UserLogin.Title + " " + item.UserLogin.Name,
                                DepartmentName = item.UserLogin.Department.Name,
                                Status = item.Status,
                                ProjectName = item.Project != null ? item.Project.Name + " (Not Found)" : "Not Found",
                                ModelName = "N/A",
                            };
                            FinalListDevelopers.Add(obj);

                        }
                    }
                }
            }
            return FinalListDevelopers;
        }


    }
}
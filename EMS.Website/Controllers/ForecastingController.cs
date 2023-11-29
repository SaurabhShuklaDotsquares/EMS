using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.LIBS;
using EMS.Web.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

using static EMS.Core.Enums;


namespace EMS.Web.Controllers
{
    [CustomAuthorization]
    public class ForecastingController : BaseController
    {
        #region "Fields"

        private IForecastingService forecastingService;
        private IDepartmentService departmentService;
        private readonly IUserLoginService userLoginService;
        #endregion

        #region "Constructor"
        public ForecastingController(IForecastingService _forecastingService, IDepartmentService _departmentService, IUserLoginService _userLoginService)
        {
            this.forecastingService = _forecastingService;
            this.departmentService = _departmentService;
            this.userLoginService = _userLoginService;
        }

        #endregion

        #region manage forecasting
        // GET: Forecasting
        public ActionResult Index()
        {
            ViewBag.ChasingStatus = typeof(Enums.ChasingStatus).EnumToDictionaryWithDescription().Select(v => new SelectListItem { Text = v.Key, Value = v.Value.ToString() }).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult GetForecastingList(IDataTablesRequest request, int status = 1, string startDate = "", string endDate = "")
        {
            string format = "dd/MM/yyyy";
            var pagingService = new PagingService<Forecasting>(request.Start, request.Length);
            var expr = PredicateBuilder.True<Forecasting>();
            pagingService.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "title":
                            return o.OrderByColumn(item, c => c.ChasingType);
                        case "crmOrLeadId":
                            return o.OrderByColumn(item, c => c.LeadId);
                        case "country":
                            return o.OrderByColumn(item, c => c.Country);
                        case "addedPerson":
                            return o.OrderByColumn(item, c => c.UserLogin.Name);
                        case "reviewed":
                            return o.OrderByColumn(item, c => c.UserLogin.Name);
                        case "noOfDeveloper":
                            return o.OrderByColumn(item, c => c.NoOfDeveloper);
                        case "addedDate":
                            return o.OrderByColumn(item, c => c.AddedDate);
                        case "tentiveDate":
                            return o.OrderByColumn(item, c => c.TentiveDate);
                        case "chasingType":
                            return o.OrderByColumn(item, c => c.ChasingType);
                        case "chasingStatus":
                            return o.OrderByColumn(item, c => c.Status);
                    }
                }

                return o.OrderByDescending(c => c.TentiveDate);
            };

            var isPm = CurrentUser.RoleId != Convert.ToInt32(Enums.UserRoles.PM) ? false : true;
            var Uid = CurrentUser.Uid;
            expr = expr.And(P => isPm ? true : P.AddedPersonUId == Uid || P.ReviewedUid.Value == Uid);
            

            if (status > 0)
            {
                expr = expr.And(P => P.Status == status);
            }
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                DateTime StartDate = DateTime.ParseExact(startDate, format, System.Globalization.CultureInfo.InvariantCulture);
                DateTime EndDate = DateTime.ParseExact(endDate, format, System.Globalization.CultureInfo.InvariantCulture);
                expr = expr.And((P => P.TentiveDate >= StartDate && P.TentiveDate <= EndDate));
            }
            else if (!string.IsNullOrEmpty(startDate))
            {
                DateTime StartDate = DateTime.ParseExact(startDate, format, System.Globalization.CultureInfo.InvariantCulture);
                expr = expr.And((P => P.TentiveDate >= StartDate));
            }
            else if (!string.IsNullOrEmpty(endDate))
            {
                DateTime EndDate = DateTime.ParseExact(endDate, format, System.Globalization.CultureInfo.InvariantCulture);
                expr = expr.And((P => P.TentiveDate <= EndDate));
            }

            pagingService.Filter = expr;

            int totalCount = 0;
            //double totalCounts = 0;
            var response = forecastingService.GetForecastingList(out totalCount, pagingService);
            //IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
            //totalCounts = forecastingService.GetTotalForecasting(pagingService);
            //additionalParameters.Add(new KeyValuePair<string, object>("totalRowCount", totalCounts));

            return DataTablesJsonResult(totalCount, request, response.Select((S, index) => new
            {
                rowId = request.Start + index + 1,
                id = S.Id,
                //title = S.ProjectLead == null ? "-" : S.ChasingType == Convert.ToInt32(Enums.ChasingType.IsExistingLead) ? S.ProjectLead.Title + (S.ProjectLead.LeadClientId != null ? " / " + S.ProjectLead.LeadClient.Name : "") : S.ProjectId != null ? S.Project.Name + (S.Project.ClientId != null ? " / " + S.Project.Client.Name : "") : S.Client.Name,
                title = S.ChasingType == Convert.ToInt32(Enums.ChasingType.IsExistingClient) ? (S.ProjectId != null ? S.Project.Name + (S.Project.ClientId != null ? " / " + S.Project.Client.Name : "") : "-") : (S.ProjectLead == null ? "-" : S.ChasingType == Convert.ToInt32(Enums.ChasingType.IsExistingLead) ? S.ProjectLead.Title + (S.ProjectLead.LeadClientId != null ? " / " + S.ProjectLead.LeadClient.Name : "") : S.ProjectId != null ? S.Project.Name + (S.Project.ClientId != null ? " / " + S.Project.Client.Name : "") : S.Client.Name),
                crmOrLeadId = S.ChasingType == Convert.ToInt32(Enums.ChasingType.IsExistingLead) ? "LEAD ID : " + S.LeadId + "" : S.ProjectId != null ? "CRM Id : " + S.Project.CRMProjectId + "" : "Client Id : " + S.ClientId + "",
                leadId = S.LeadId,
                projectId = S.ProjectId != null ? S.Project.CRMProjectId : 0,
                country = S.Country,
                addedPerson = S.UserLogin.Name,
                addedPersonUID = S.UserLogin.Uid,
                reviewed = S.ReviewedU?.Name,
                reviewedUId = S.ReviewedUid,
                addedDate = S.AddedDate.ToString("MMM dd, yyyy"),
                infoColor = S.IsHold.HasValue ? (S.IsHold.Value ? "color:red" : "color:black") : "color:black",
                chasingType = S.ChasingType == Convert.ToInt32(Enums.ChasingType.IsExistingClient) ? "Existing Client" : "Existing Lead",
                tentiveDate = string.Format("{0:MMM dd, yyyy}", S.TentiveDate.Value),
                projectDescription = S.IsHold.HasValue ? (S.IsHold.Value ? S.HoldReason : S.ProjectDescription) : S.ProjectDescription,
                //   Technologys = GetForecastingGroups(S.Id),
                noOfDeveloper = S.NoOfDeveloper != null ? S.NoOfDeveloper : 0,
                chasingStatus = S.Status == Convert.ToInt32(Enums.ChasingStatus.Pending) ? "Pending" : "Converted",
                isUserRolePM = isPm
            }), null);
        }

        [HttpPost]
        public JsonResult DeleteRecord(int id)
        {
            try
            {
                forecastingService.Delete(id);
                //ShowSuccessMessage("Success!", "Record deleted successfully", false);
                return new JsonResult(new { status = true });

                //return new JsonResult { Data = new { status = true } };
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false });
            }
        }

        [HttpPost]
        public JsonResult ChangeStatus(int id, int status)
        {
            try
            {
                forecastingService.ChangeStatus(id, status);
                //ShowSuccessMessage("Success!", "Record updated successfully", false);
                //return new JsonResult { Data = new { status = true } };
                return new JsonResult(new { status = true });

            }
            catch (Exception ex)
            {
                // return new JsonResult { Data = new { status = false } };
                return new JsonResult(new { status = false });

            }
        }
        #endregion manage forecasting

        #region create forecasting
        public ActionResult ManageForecasting()
        {
            ManageForecastingDto objManageForecastingDto = new ManageForecastingDto();
            ViewBag.LeadList = GetChasingLeads();
            ViewBag.CountryList = GetCountryList();
            ViewBag.Projectlist = GetProjects();
            objManageForecastingDto.DepartmentList = GetDepartment(true);
            return View(objManageForecastingDto);
        }

        [HttpPost]
        public ActionResult ManageForecasting(ManageForecastingDto objManageForecastingDto)
        {
            try
            {
                string format = "dd/MM/yyyy";
                DateTime TentiveDate = DateTime.ParseExact(objManageForecastingDto.TentiveDate, format, System.Globalization.CultureInfo.InvariantCulture); bool IsEdit = (Convert.ToInt64(objManageForecastingDto.Id) > 0) ? true : false;
                Forecasting forecastingDB = new Forecasting();
                List<Department> lstForecasting_Department = new List<Department>();

                if (IsEdit)
                {
                    forecastingDB = forecastingService.GetForecastingById(objManageForecastingDto.Id);
                    forecastingDB.ProjectDescription = objManageForecastingDto.ProjectDescription;
                    forecastingDB.LeadId = Convert.ToInt32(objManageForecastingDto.Lead);
                    forecastingDB.Country = objManageForecastingDto.Country;
                    forecastingDB.TentiveDate = TentiveDate;
                    forecastingDB.ChasingType = objManageForecastingDto.ForecastingType;
                    forecastingDB.NoOfDeveloper = Convert.ToInt32(objManageForecastingDto.DeveloperCount);
                    forecastingDB.AddedPersonUId = objManageForecastingDto.OwnerUId;
                    forecastingDB.ReviewedUid = objManageForecastingDto.ReviewedUId;
                    ShowSuccessMessage("Success!", "Record updated successfully", false);
                }
                else
                {
                    forecastingDB.ProjectDescription = objManageForecastingDto.ProjectDescription;
                    forecastingDB.Country = objManageForecastingDto.Country;
                    forecastingDB.TentiveDate = TentiveDate;
                    forecastingDB.ChasingType = objManageForecastingDto.ForecastingType;
                    forecastingDB.Groups = "";
                    forecastingDB.NoOfDeveloper = Convert.ToInt32(objManageForecastingDto.DeveloperCount);
                    forecastingDB.Status = Convert.ToInt32(Enums.ChasingStatus.Pending);
                    forecastingDB.AddedPersonUId = objManageForecastingDto.OwnerUId;
                    forecastingDB.ReviewedUid = objManageForecastingDto.ReviewedUId;
                    forecastingDB.AddedDate = DateTime.Now;
                    forecastingDB.IsHold = objManageForecastingDto.IsHold;
                    forecastingDB.HoldReason = objManageForecastingDto.IsHold ? objManageForecastingDto.HoldReason : null;
                    //todo
                    //for lead 
                    if (objManageForecastingDto.ForecastingType == 1)
                    {
                        forecastingDB.LeadId = Convert.ToInt32(objManageForecastingDto.Lead);
                    }
                    //for project 
                    if (objManageForecastingDto.ForecastingType == 2 && !string.IsNullOrEmpty(objManageForecastingDto.ProjectId))
                    {
                        string[] projectorclient = objManageForecastingDto.ProjectId.Split(',');
                        if (projectorclient[0] == "Project")
                            forecastingDB.ProjectId = Convert.ToInt32(projectorclient[1]);
                        else
                            forecastingDB.ClientId = Convert.ToInt32(projectorclient[1]);
                    }
                    //insert department
                    foreach (var row in objManageForecastingDto.DepartmentList.Where(x => x.Selected == true).ToList())
                    {

                        forecastingDB.ForecastingDepartment.Add(new ForecastingDepartment { Forecasting = forecastingDB, Department = departmentService.DepartmentFindById(Convert.ToInt32(row.Value)) });

                        // forecastingDB.Departments.Add(departmentService.DepartmentFindById(Convert.ToInt32(row.Value)));
                    }
                    forecastingService.SaveForecasting(forecastingDB);
                    ShowSuccessMessage("Success!", "Record saved successfully", false);
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index");
        }

        public ActionResult _ManageForecasting()
        {
            ManageForecastingDto objManageForecastingDto = new ManageForecastingDto();
            ViewBag.LeadList = GetChasingLeads();
            ViewBag.CountryList = GetCountryList();
            ViewBag.Projectlist = GetProjects();
            ViewBag.UserList = GetUserList();
            objManageForecastingDto.DepartmentList = GetDepartment(true);
            return PartialView(objManageForecastingDto);
        }

        [HttpPost]
        public string GetChasingLeadsAutoComplete(string prefix)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(prefix))
                {
                    prefix = prefix.ToLower();
                    var projectLeads = GetChasingLeads(prefix);
                    var componentList = projectLeads.Where(x => x.Text.IndexOf(prefix, StringComparison.InvariantCultureIgnoreCase) > -1)
                        .Select(x => new { Text = x.Text, Value = x.Value }).ToArray();
                    //var componentList = (from c in projectLeads
                    //                     where c.Text.IndexOf(prefix, StringComparison.InvariantCultureIgnoreCase) > -1
                    //                     select c.Text ).ToArray();
                    return JsonConvert.SerializeObject(componentList);

                }
            }
            catch (Exception ex)
            {

            }
            return "";
        }


        public ActionResult EditManageForecasting(int id)
        {
            ManageForecastingDto objManageForecastingDto = new ManageForecastingDto();
            ViewBag.LeadList = GetChasingLeads();
            ViewBag.CountryList = GetCountryList();
            ViewBag.Projectlist = GetProjects();
            objManageForecastingDto.DepartmentList = GetDepartment(true);
            if (id > 0)
            {
                var forecastingDB = forecastingService.GetForecastingById(id);
                objManageForecastingDto.Id = forecastingDB.Id;
                objManageForecastingDto.ProjectDescription = forecastingDB.ProjectDescription;
                objManageForecastingDto.Country = forecastingDB.Country;
                objManageForecastingDto.TentiveDate = forecastingDB.TentiveDate.Value.ToString("dd/MM/yyyy").Replace('-', '/');
                objManageForecastingDto.ForecastingType = forecastingDB.ChasingType;
                objManageForecastingDto.IsHold = !forecastingDB.IsHold.HasValue ? false : forecastingDB.IsHold.Value;
                objManageForecastingDto.HoldReason = forecastingDB.HoldReason;
                objManageForecastingDto.DeveloperCount = forecastingDB.NoOfDeveloper.ToString();
                //todo
                //for lead 
                if (forecastingDB.ChasingType == 1)
                {
                    objManageForecastingDto.Lead = forecastingDB.LeadId.Value.ToString();
                }
                //for project 
                if (forecastingDB.ChasingType == 2)
                {
                    if (forecastingDB.ProjectId > 0)
                        objManageForecastingDto.ProjectId = "Project," + forecastingDB.ProjectId;
                    else if (forecastingDB.ClientId > 0)
                        objManageForecastingDto.ProjectId = "Client," + forecastingDB.ClientId;
                }
                //foreach (Department dept in forecastingDB.Departments)
                //{
                //    foreach (var item in objManageForecastingDto.DepartmentList)
                //    {
                //        if (dept.DeptId == Convert.ToInt32(item.Value))
                //        {
                //            item.Selected = true;
                //            break;
                //        }

                //    }
                //}
                foreach (ForecastingDepartment dept in forecastingDB.ForecastingDepartment)
                {
                    foreach (var item in objManageForecastingDto.DepartmentList)
                    {
                        if (dept.DepartmentId == Convert.ToInt32(item.Value))
                        {
                            item.Selected = true;
                            break;
                        }

                    }
                }
            }
            return View(objManageForecastingDto);
        }
        public ActionResult _EditManageForecasting(int id)
        {
            ManageForecastingDto objManageForecastingDto = new ManageForecastingDto();
            ViewBag.LeadList = GetChasingLeads();
            ViewBag.CountryList = GetCountryList();
            ViewBag.Projectlist = GetProjects();
            ViewBag.UserList = GetUserList();
            objManageForecastingDto.DepartmentList = GetDepartment(true);
            if (id > 0)
            {
                var forecastingDB = forecastingService.GetForecastingById(id);
                objManageForecastingDto.Id = forecastingDB.Id;
                objManageForecastingDto.ProjectDescription = forecastingDB.ProjectDescription;
                objManageForecastingDto.Country = forecastingDB.Country;
                objManageForecastingDto.TentiveDate = forecastingDB.TentiveDate.Value.ToString("dd/MM/yyyy").Replace('-', '/');
                objManageForecastingDto.ForecastingType = forecastingDB.ChasingType;
                objManageForecastingDto.OwnerUId = forecastingDB.AddedPersonUId;
                objManageForecastingDto.ReviewedUId = forecastingDB.ReviewedUid;
                objManageForecastingDto.DeveloperCount = forecastingDB.NoOfDeveloper.ToString();

                objManageForecastingDto.IsHold = !forecastingDB.IsHold.HasValue ? false : forecastingDB.IsHold.Value;
                objManageForecastingDto.HoldReason = objManageForecastingDto.IsHold ? forecastingDB.HoldReason : null;
               
                //todo
                //for lead 
                if (forecastingDB.ChasingType == 1)
                {
                    objManageForecastingDto.Lead = forecastingDB.LeadId.Value.ToString();
                }
                //for project 
                if (forecastingDB.ChasingType == 2)
                {
                    if (forecastingDB.ProjectId > 0)
                        objManageForecastingDto.ProjectId = "Project," + forecastingDB.ProjectId;
                    else if (forecastingDB.ClientId > 0)
                        objManageForecastingDto.ProjectId = "Client," + forecastingDB.ClientId;
                }
                //foreach (Department dept in forecastingDB.Departments)
                //{
                //    foreach (var item in objManageForecastingDto.DepartmentList)
                //    {
                //        if (dept.DeptId == Convert.ToInt32(item.Value))
                //        {
                //            item.Selected = true;
                //            break;
                //        }

                //    }
                //}
                foreach (ForecastingDepartment dept in forecastingDB.ForecastingDepartment)
                {
                    foreach (var item in objManageForecastingDto.DepartmentList)
                    {
                        if (dept.DepartmentId == Convert.ToInt32(item.Value))
                        {
                            item.Selected = true;
                            break;
                        }

                    }
                }
            }
            return PartialView(objManageForecastingDto);
        }
        [HttpPost]
        public ActionResult EditManageForecasting(ManageForecastingDto objManageForecastingDto)
        {
            try
            {
                string format = "dd/MM/yyyy";
                DateTime TentiveDate = DateTime.ParseExact(objManageForecastingDto.TentiveDate, format, System.Globalization.CultureInfo.InvariantCulture);
                Forecasting forecastingDB = new Forecasting();
                forecastingDB = forecastingService.GetForecastingById(objManageForecastingDto.Id);

                forecastingDB.ProjectDescription = objManageForecastingDto.ProjectDescription;
                forecastingDB.Country = objManageForecastingDto.Country;
                forecastingDB.TentiveDate = TentiveDate;
                forecastingDB.ChasingType = objManageForecastingDto.ForecastingType;
                forecastingDB.NoOfDeveloper = Convert.ToInt32(objManageForecastingDto.DeveloperCount);
                forecastingDB.IsHold = objManageForecastingDto.IsHold;
                forecastingDB.HoldReason = objManageForecastingDto.IsHold ? objManageForecastingDto.HoldReason : null;
                forecastingDB.AddedPersonUId = objManageForecastingDto.OwnerUId;
                forecastingDB.ReviewedUid = objManageForecastingDto.ReviewedUId;
                //todo
                //for lead 
                if (objManageForecastingDto.ForecastingType == 1)
                {
                    forecastingDB.LeadId = Convert.ToInt32(objManageForecastingDto.Lead);
                }
                //for project 
                if (objManageForecastingDto.ForecastingType == 2 && !string.IsNullOrEmpty(objManageForecastingDto.ProjectId))
                {
                    string[] projectorclient = objManageForecastingDto.ProjectId.Split(',');
                    if (projectorclient[0] == "Project")
                        forecastingDB.ProjectId = Convert.ToInt32(projectorclient[1]);
                    else
                        forecastingDB.ClientId = Convert.ToInt32(projectorclient[1]);
                }
                //delete exits department
                //foreach (var row in forecastingDB.Departments)
                //{
                //    forecastingDB.Departments.Clear()(departmentService.DepartmentFindById(Convert.ToInt32(row.DeptId)));
                //}
                forecastingDB.ForecastingDepartment.Clear();

                //forecastingDB.Departments.Clear();

                //insert department
                foreach (var row in objManageForecastingDto.DepartmentList.Where(x => x.Selected == true).ToList())
                {
                    //   forecastingDB.Departments.Add(departmentService.DepartmentFindById(Convert.ToInt32(row.Value)));

                    forecastingDB.ForecastingDepartment.Add(new ForecastingDepartment { Department = departmentService.DepartmentFindById(Convert.ToInt32(row.Value)) });


                }
                forecastingService.SaveForecasting(forecastingDB);
                ShowSuccessMessage("Success!", "Record updated successfully", false);
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index");
        }
        #endregion create forecasting

        private List<SelectListItem> GetDepartment(bool IsActive = false)
        {
            var deptList = forecastingService.GetDepartment();
            var list = deptList.Where(G => (G.IsActive == true || G.IsActive == IsActive)).Select(x => new SelectListItem
            {
                Text = x.Name.ToString(),
                Value = x.DeptId.ToString(),
            }).ToList();
            return list;
        }
        private List<SelectListItem> GetChasingLeads()
        {
            var leadList = forecastingService.GetProjectLeads();
            var list = leadList.Where(L => L.Status != 2).Select(x => new SelectListItem
            {
                Text = x.Title + " (LEAD ID : " + x.LeadId + ")",
                Value = x.LeadId.ToString(),
                Selected = false,
            }).OrderBy(T=>T.Text).ToList();
            return list;
        }
        private List<SelectListItem> GetChasingLeads(string prefix)
        {
            var leadList = forecastingService.GetProjectLeads();
            var list = leadList.Where(L => L.Status != 2 && L.Title.ToLower().Contains(prefix)).Select(x => new SelectListItem
            {
                Text = x.Title + " (LEAD ID : " + x.LeadId + ")",
                Value = x.LeadId.ToString(),
                Selected = false,
            }).ToList();
            return list;
        }

        private List<SelectListItem> GetProjects()
        {
            List<Project> lsProject = forecastingService.GetProjects().Where(x => x.PMUid.Equals(CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.PMUid)).ToList();
            List<String> lsClientId = new List<string>();
            foreach (var item in lsProject)
            {
                if (item.ClientId != null)
                    lsClientId.Add(item.ClientId.ToString());
            }
            List<Client> lsClient = forecastingService.GetClientData(lsClientId.ToArray());
            var res = lsProject;
            //bind client
            var resClient = lsClient;
            var objClient = resClient.Select(P => new ClientDto
            {
                ProjectName = P.Name,
                CrmId = 0,
                PMUid = P.PMUid,
                ProjectId = P.ClientId,
                ClientOrProjectId = "Client," + P.ClientId
            });

            //bind project
            var obj = res.Select(P => new ClientDto
            {
                ProjectName = P.Name + "" + (P.ClientId != null ? " (CRM Id : " + P.CRMProjectId + ") - " + P.Client.Name : ""),
                CrmId = P.CRMProjectId,
                PMUid = P.PMUid,
                ProjectId = P.ProjectId,
                ClientOrProjectId = "Project," + P.ProjectId
            });
           // obj = obj.Concat(objClient);


            var list = obj.Select(x => new SelectListItem
            {
                Text = x.ProjectName,
                Value = x.ClientOrProjectId.ToString(),
            }).OrderBy(T => T.Text).ToList();
            return list;
        }

        private List<SelectListItem> GetCountryList()
        {
            var countryList = new List<KeyValuePair<string, string>>();
            countryList.Add(new KeyValuePair<string, string>("UK", "UK"));
            countryList.Add(new KeyValuePair<string, string>("US", "US"));
            countryList.Add(new KeyValuePair<string, string>("AUS", "AUS"));
            countryList.Add(new KeyValuePair<string, string>("IND", "IND"));


            var returnTypeList = new List<CommonListType>();
            var list = countryList.Select(x => new SelectListItem
            {
                Text = x.Key.ToString(),
                Value = x.Value.ToString(),
            }).ToList();
            return list;
        }

        private List<SelectListItem> GetUserList()
        {
            var userList = userLoginService.GetUsersByPM(PMUserId);

            //model.BAList = userList.Where(x => x.RoleId == (int)Enums.UserRoles.BA || x.RoleId == (int)Enums.UserRoles.PM)
            //                       .ToSelectList(x => x.Name, x => x.Uid);
            //model.TLList = userList.Where(x => x.RoleId == (int)Enums.UserRoles.TL || x.RoleId == (int)Enums.UserRoles.SD || x.RoleId == (int)Enums.UserRoles.TLS)
            //                       .ToSelectList(x => x.Name, x => x.Uid);
            return userList.ToSelectList(x => x.Name, x => x.Uid);
        }

        //private void BindData()
        //{

        //    List<Project> lsProject = Project.GetProjects().Where(x => x.PMUid.Equals(SiteSession.SessionUser.RoleId == (int)Enums.UserRoles.PM
        //        ? SiteSession.SessionUser.Uid : SiteSession.SessionUser.PMUid)).ToList();

        //    List<String> lsClientId = new List<string>();
        //    foreach (var item in lsProject)
        //    {
        //        if (item.ClientId != null)
        //            lsClientId.Add(item.ClientId.ToString());
        //    }

        //    List<Client> lsClient = Client.GetClientData(lsClientId.ToArray());
        //    var res = lsProject;

        //    //bind client
        //    var resClient = lsClient;
        //    IEnumerable<object> objClient = resClient.Select(P => new Project
        //    {
        //        ProjectName = P.Name,
        //        CrmId = 0,
        //        PMUid = P.PMUid,
        //        ProjectId = P.ClientId,
        //        ClientOrProjectId = "Client," + P.ClientId
        //    });

        //    //bind project
        //    IEnumerable<object> obj = res.Select(P => new Project
        //    {
        //        ProjectName = P.Name + "" + (P.ClientId != null ? " (CRM Id : " + P.CRMProjectId + ") - " + P.Client.Name : ""),
        //        CrmId = P.CRMProjectId,
        //        PMUid = P.PMUid,
        //        ProjectId = P.ProjectId,
        //        ClientOrProjectId = "Project," + P.ProjectId
        //    });

        //    obj = obj.Concat(objClient);
        //    Common.BindControl(ddlProjects, obj, "ProjectName", "ClientOrProjectId", "Select Project / Client");
        //    chkGroup.DataSource = Department.GetDepartment(true).OrderBy(G => G.Name);
        //    chkGroup.DataTextField = "Name";
        //    chkGroup.DataValueField = "DeptID";
        //    chkGroup.DataBind();
        //}
    }
}
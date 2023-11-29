using EMS.Service;
using EMS.Web.Code.LIBS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EMS.Core;
using EMS.Data;
using EMS.Web.Code.Attributes;
using Microsoft.AspNetCore.Mvc;
using EMS.Dto;
using EMS.Data.Model;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class ProjectInfoController : BaseController
    {

        private readonly IUserLoginService userLoginService;
        private readonly IProjectService projectService;
        private readonly IVirtualDeveloperService virtualDeveloperService;
        private readonly IBucketModelService bucketModelService;
        private readonly ITechnologyService technologyService;



        public ProjectInfoController(IUserLoginService userLoginService, IProjectService projectService, IVirtualDeveloperService virtualDeveloperService, IBucketModelService bucketModelService, ITechnologyService technologyService)
        {
            this.userLoginService = userLoginService;
            this.projectService = projectService;
            this.virtualDeveloperService = virtualDeveloperService;
            this.bucketModelService = bucketModelService;
            this.technologyService = technologyService;
        }

        // GET: ProjectInfo
        [CustomActionAuthorization]
        public ActionResult Index(string id)
        {
            try
            {
                //JavaScriptSerializer jss = new JavaScriptSerializer();

                ProjectsUpdateDto model = new ProjectsUpdateDto();


                //List<ProjectInfoDto> Lsproject = new List<ProjectInfoDto>();

                int CrmUserid = 0;
                string ApiPass = "";
                int pmuid = 0;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    string pmid = EncryptDecrypt.Decrypt(id);
                    int.TryParse(pmid, out pmuid);
                }
                else
                {
                    pmuid = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;
                }
                model.PmUidEncrypted = id;
                model.PmUid = pmuid;
                var userData = userLoginService.GetUserInfoByID(pmuid);
                if (userData != null)
                {
                    CrmUserid = userData.CRMUserId != null ? userData.CRMUserId.Value : 0;
                    ApiPass = String.IsNullOrEmpty(userData.ApiPassword) ? "" : userData.ApiPassword;
                }
                if (CrmUserid == 0 && ApiPass == "")
                {
                    ShowErrorMessage("Error", "User Id and password is wrong.", true);
                    return View(new List<ProjectInfoDto>());
                }


                string jsonString = new WebClient().DownloadString(SiteKey.CrmProjectList + CrmUserid + "&apipass=" + ApiPass);
                //string jsonString = new WebClient().DownloadString("https://dsc.dotsquares.com/index.php/getprojectstatus/projectDetail?type=projects&userid=238&apipass=832");

                // var jsonString = System.IO.File.ReadAllText("D:\\local\\DSManage\\DotsquaresManagement\\EMS.Web\\Crmdata.txt");
                jsonString = jsonString.Replace("\"virtual\":", "\"virtual_dev\":");
                jsonString = jsonString.Replace("\"dev_detail\":\"\"", "\"dev_detail\":[{}]");//\"virtual_dev\":{\"username\":\"\",\"email\":\"\"}
                JObject obj = JObject.Parse(jsonString);
                var token = (JArray)obj.SelectToken("response");
                string json = null;
                //string dbnotes = "";
                foreach (var item in token)
                {
                    json = JsonConvert.SerializeObject(item.SelectToken("result"));
                }
                if (CrmUserid == 0 && ApiPass == "")
                {
                    ShowErrorMessage("Error", "No record found.", true);
                    return View();
                }
                if (json != null)
                {
                    //jss.MaxJsonLength = Int32.MaxValue;
                    //ViewState.Add("Crmlist", json);
                    List<ProjectInfoDto> CRMProjectList = JsonConvert.DeserializeObject<List<ProjectInfoDto>>(json).ToList();

                    if (CRMProjectList != null && CRMProjectList.Any())
                    {

                        #region "Get EMS Projects"

                        /*Project.GetProjectsCompByPMUid(pmuid);*/


                        var EMSProjects = projectService.GetProjectWithOtherPMByPmuid(pmuid);


                        var EMSProjectList = new List<ProjectInfoDto>();

                        //UserLogin.GetUsers(EMSProjects.Where(x => x.AbroadPMUid.HasValue && x.AbroadPMUid.Value > 0)
                        //                                      .Select(x => x.AbroadPMUid.Value).Distinct().ToList());

                        var userIds = EMSProjects.Where(x => x.AbroadPMUid.HasValue && x.AbroadPMUid.Value > 0)
                                                                  .Select(x => x.AbroadPMUid.Value).Distinct().ToList();

                        var pmUsers = userLoginService.GetUsersListByUserIds(userIds);

                        foreach (var prj in EMSProjects)
                        {
                            EMSProjectList.Add(new ProjectInfoDto()
                            {
                                Project_Id = Convert.ToString(prj.ProjectId),
                                EmsCRMPId = Convert.ToString(prj.CRMProjectId),
                                Project_Name = prj.Name,
                                Project_Status = prj.Status,
                                Developer = Convert.ToString(prj.ActualDevelopers),
                                ModelName = prj.BucketModel?.ModelName ?? "",
                                WorkingDevelopers = prj.ProjectDevelopers.Count(d => d.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running),
                                Bill_Team = prj.BillingTeam,
                                Estimate_Time = Convert.ToString(prj.EstimateTime),
                                Start_Date = Convert.ToDateTime(prj.StartDate),
                                Client_Name = prj.Client?.Name ?? "",
                                ProjectDevelopers = prj.ProjectDevelopers != null ? prj.ProjectDevelopers.Select(p =>
                                new Proj_DevDetail
                                {
                                    ProjectDeveloperID = p.ProjectDeveloperId,
                                    WorkStatus = p.WorkStatus,
                                    VD_Id = p.VD_id,
                                    AD_Id = p.Uid
                                }).ToList() : null,
                                Pm_Email = pmUsers.FirstOrDefault(u => u.Uid == (prj.AbroadPMUid ?? 0))?.EmailOffice ?? "",
                                PMUId = prj.PMUid,
                                ProjectOtherContactPMList = prj.ProjectOtherPm != null ? prj.ProjectOtherPm.Select(PO => new ProjectOtherContactPM { PMUid = PO.Pmuid, ProjectId = PO.ProjectId }).ToList() : new List<ProjectOtherContactPM>()
                            });
                        }

                        #endregion

                       // List<UserLogin> userlist = userLoginService.GetUserByUKBDMUid(pmuid) ?? new List<UserLogin>();

                        List<UserLogin> userlist = userLoginService.GetUsers(true) ?? new List<UserLogin>();

                        List<VirtualDeveloper> virtual_devList = virtualDeveloperService.GetVirtualDeveloperByPMUid(pmuid) ?? new List<VirtualDeveloper>();
                        /*VirtualDeveloper.GetVirtualDeveloperByPMUid(pmuid) ?? new List<VirtualDeveloper>();*/

                        var crmidList = EMSProjectList.Select(X => X.EmsCRMPId);

                        // Hard Coded Project CRM Id for Testing
                        // crmidList = EMSProjectList.Where(p => p.EmsCRMPId == "1012").Select(X => X.EmsCRMPId);

                        var ProjectList1 = CRMProjectList.Where(p => crmidList.Contains(p.Project_Id)).ToList();

                        foreach (var itemCRM in ProjectList1)
                        {
                            var itemEMS = EMSProjectList.FirstOrDefault(x => x.EmsCRMPId == itemCRM.Project_Id);

                            if (itemEMS != null)
                            {
                                try
                                {
                                    string actual_dev_email = string.Empty;
                                    string virtual_dev_email = string.Empty;

                                    string actual_devID = string.Empty;
                                    string virtual_devID = string.Empty;

                                    string Emsvirtual_dev_ID = string.Empty;
                                    string Emsactual_dev_ID = string.Empty;

                                    if (itemEMS.EmsCRMPId == itemCRM.Project_Id)
                                    {
                                        List<Proj_DevDetail> list = itemEMS.ProjectDevelopers.Where(x => x.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running).ToList();

                                        if (list != null && list.Any())
                                        {
                                            var ListId = list.Select(X => X.VD_Id).ToList();

                                            for (int v = 0; v <= ListId.Count - 1; v++)
                                            {
                                                var vid = virtual_devList.FirstOrDefault(a => a.VirtualDeveloper_ID == Convert.ToInt32(ListId[v]) && a.isactive == true);
                                                Emsvirtual_dev_ID += vid != null ? vid.VirtualDeveloper_ID.ToString() + "," : "";
                                            }

                                            var listADId = list.Select(X => X.AD_Id).ToList();

                                            for (int v = 0; v <= listADId.Count - 1; v++)
                                            {
                                                var adid = userlist.FirstOrDefault(a => a.Uid == Convert.ToInt32(listADId[v]) && a.IsActive == true);
                                                Emsactual_dev_ID += adid != null ? adid.Uid.ToString() + "," : "";
                                            }
                                        }

                                        string strdevdetails = "";
                                        if (itemCRM.Dev_Detail != null && itemCRM.Dev_Detail.Any())
                                        {
                                            for (int i = 0; i <= itemCRM.Dev_Detail.Count - 1; i++)
                                            {
                                                int actid = 0;
                                                int vdevid = 0;
                                                actual_dev_email = string.Empty;
                                                if (itemCRM.Dev_Detail[i].Actual != null)
                                                {
                                                    try
                                                    {
                                                        actual_dev_email = (itemCRM.Dev_Detail[i].Actual.Email ?? "").ToLower().Trim();
                                                    }
                                                    catch
                                                    {
                                                        actual_dev_email = "";
                                                    }
                                                }

                                                virtual_dev_email = string.Empty;
                                                if (itemCRM.Dev_Detail[i].Virtual_Dev != null)
                                                {
                                                    try
                                                    {
                                                        virtual_dev_email = (itemCRM.Dev_Detail[i].Virtual_Dev.Email ?? "").ToLower().Trim();
                                                    }
                                                    catch
                                                    {
                                                        virtual_dev_email = "";
                                                    }
                                                }

                                                if (userlist != null && virtual_devList != null)
                                                {
                                                    if (!string.IsNullOrEmpty(actual_dev_email) && userlist.Select(X => X.EmailOffice.ToLower()).Contains(actual_dev_email) == true)
                                                    {
                                                        var acdevid = userlist.FirstOrDefault(R => R.EmailOffice.ToLower().Trim() == actual_dev_email && R.IsActive == true);
                                                        actual_devID += (acdevid != null ? (acdevid.Uid.ToString() + ",") : "");
                                                        actid = (acdevid != null ? acdevid.Uid : 0);
                                                    }

                                                    if (!string.IsNullOrEmpty(virtual_dev_email))
                                                    {
                                                        bool isvDev = virtual_devList.Any(a => a.emailid.ToLower().Trim() == virtual_dev_email && a.isactive == true);
                                                        if (isvDev == false)
                                                        {
                                                            // IF developer not exist in system then first we will add in system 
                                                            string vname = string.Empty;
                                                            try
                                                            {
                                                                vname = virtual_dev_email.Split('@')[0].Trim().Replace(".", " ");
                                                            }
                                                            catch { }

                                                            var virtualDeveloper = new VirtualDeveloper
                                                            {
                                                                VirtualDeveloper_Name = vname,
                                                                SkypeId = "",
                                                                isactive = true,
                                                                emailid = virtual_dev_email,
                                                                Ismain = true,
                                                                PMUid = pmuid,
                                                                ModifiedDate = DateTime.Now,
                                                                CreationDate = DateTime.Now
                                                            };

                                                            virtualDeveloperService.Save(virtualDeveloper);

                                                            virtual_devList = virtualDeveloperService.GetVirtualDeveloperByPMUid(pmuid);
                                                            //VirtualDeveloper.GetVirtualDeveloperByPMUid(pmuid);
                                                        }
                                                        var vdevlist = virtual_devList.FirstOrDefault(a => a.emailid.ToLower().Trim() == virtual_dev_email && a.isactive == true);
                                                        if (vdevlist != null)
                                                        {
                                                            virtual_devID += vdevlist.VirtualDeveloper_ID.ToString() + ",";
                                                            vdevid = vdevlist.VirtualDeveloper_ID;
                                                        }
                                                    }
                                                }
                                                if (vdevid != 0)
                                                {
                                                    strdevdetails += actid + ":" + vdevid + ",";
                                                }
                                            }
                                        }

                                        strdevdetails = strdevdetails.TrimEnd(',');
                                        itemCRM.Pdev_Detail = strdevdetails;

                                        string[] _emsVID = (!string.IsNullOrEmpty(Emsvirtual_dev_ID) ? Emsvirtual_dev_ID.TrimEnd(',').Split(',') : null);
                                        string[] _crmVID = (!string.IsNullOrEmpty(virtual_devID) ? virtual_devID.TrimEnd(',').Split(',') : null);

                                        int flag = 0;
                                        if (Convert.ToInt32(itemCRM.Developer) == 0 && (itemCRM.Project_Status == "Runing" || itemCRM.Project_Status == "Running"))
                                        {
                                            itemCRM.Project_Status = "On Hold";
                                        }

                                        string cstatus = Common.GetEMSProjectStatus(itemCRM.Project_Status.ToLower().Trim());
                                        string cstatusdetail = itemCRM.Project_Status.Trim();

                                        // Check exist condition for virtual developer
                                        if (_emsVID != null && _crmVID != null && _emsVID.Any() && _crmVID.Any())
                                        {
                                            if (_emsVID.Count() == _crmVID.Count())
                                            {

                                                for (int k = 0; k <= _crmVID.Count() - 1; k++)
                                                {
                                                    if (!_crmVID.Contains(_emsVID[k]))
                                                    {
                                                        flag++;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                flag++;
                                            }
                                        }
                                        else if (_emsVID != _crmVID)
                                        {
                                            if (!((itemEMS.Project_Status.ToUpper() == "C" || itemEMS.Project_Status.ToUpper() == "H" || itemEMS.Project_Status.ToUpper() == "D" || itemEMS.Project_Status.ToUpper() == "N") &&
                                                itemEMS.Project_Status.ToUpper() == cstatus.ToUpper()))
                                            {
                                                flag++;
                                            }
                                        }

                                        if (flag == 0)
                                        {
                                            string[] _emsADID = (!string.IsNullOrEmpty(Emsactual_dev_ID) ? Emsactual_dev_ID.TrimEnd(',').Split(',') : null);
                                            string[] _crmADID = (!string.IsNullOrEmpty(actual_devID) ? actual_devID.TrimEnd(',').Split(',') : null);

                                            // Check exist condition for actual developer
                                            if (_emsADID != null && _crmADID != null && _emsADID.Any() && _crmADID.Any())
                                            {

                                                if (_emsADID.Count() == _crmADID.Count())
                                                {
                                                    for (int k = 0; k <= _crmADID.Count() - 1; k++)
                                                    {
                                                        if (!_crmADID.Contains(_emsADID[k]))
                                                        {
                                                            flag++;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    flag++;
                                                }
                                            }
                                            else if (_emsADID != _crmADID)
                                            {
                                                if (!((itemEMS.Project_Status.ToUpper() == "C" || itemEMS.Project_Status.ToUpper() == "H" || itemEMS.Project_Status.ToUpper() == "D" || itemEMS.Project_Status.ToUpper() == "N") &&
                                                    itemEMS.Project_Status.ToUpper() == cstatus.ToUpper()))
                                                {
                                                    flag++;
                                                }
                                            }
                                        }

                                        string crmPMEmail = string.IsNullOrEmpty(itemCRM.Pm_Email) ? "" : Convert.ToString(itemCRM.Pm_Email);

                                        string Bteam = Common.GetBillingTeam(itemCRM.Bill_Team.ToLower().Trim());
                                        if (flag > 0 || itemCRM.Developer != Convert.ToString(itemEMS.Developer) || itemCRM.ModelName.ToLower().Trim() != itemEMS.ModelName.ToLower().Trim() ||
                                            cstatus.ToLower().Trim() != itemEMS.Project_Status.ToLower().Trim() || Bteam.ToLower().Trim() != itemEMS.Bill_Team.ToLower().Trim() ||
                                            itemCRM.Start_Date != itemEMS.Start_Date || (itemEMS.Client_Name == "" && itemCRM.Client_Name != "")
                                            || ((itemEMS.Pm_Email != crmPMEmail) 
                                            &&  (itemCRM.ModelName.ToLower().Trim() == "blended -pm from uk office" ||
                                            itemCRM.ModelName.ToLower().Trim() == "bucket system- blended" ||
                                            itemCRM.ModelName.ToLower().Trim() == "uk pm support" || itemCRM.ModelName.ToLower().Trim() == "bucket system- blended uk")))
                                        //&& cstatus == "R" &&
                                        {
                                            itemCRM.IsNew = false;
                                            itemCRM.IsRunDev = false;
                                            itemCRM.WorkingDevelopers = itemEMS.WorkingDevelopers;
                                            itemCRM.Modifiydate = itemEMS.Modifiydate;
                                            itemCRM.Virtual_DevId = virtual_devID;
                                            itemCRM.Actual_DevId = actual_devID;
                                            itemCRM.Bill_Team = Bteam;


                                            model.ProjectsListInfo.Add(itemCRM);
                                        }
                                        else if (itemCRM.Developer != Convert.ToString(itemEMS.WorkingDevelopers) && itemEMS.WorkingDevelopers > 0)
                                        {
                                            itemCRM.IsRunDev = true;
                                            itemCRM.IsNew = false;
                                            itemCRM.WorkingDevelopers = itemEMS.WorkingDevelopers;
                                            itemCRM.Modifiydate = itemEMS.Modifiydate;
                                            itemCRM.Virtual_DevId = virtual_devID;
                                            itemCRM.Actual_DevId = actual_devID;
                                            itemCRM.Bill_Team = Bteam;

                                            // hidecounter++;
                                            model.ProjectsListInfo.Add(itemCRM);
                                        }



                                        // Add project in update list if changed Assigned PM and other PM

                                        if (itemCRM.other_contact.ToLower() == "yes" && !model.ProjectsListInfo.Any(x => x.Project_Id == itemCRM.Project_Id))
                                        {
                                            //if (itemEMS.PMUId == pmuid)
                                            if (!itemEMS.ProjectOtherContactPMList.Any(p => p.PMUid == pmuid))
                                            {
                                                itemCRM.IsNew = false;
                                                itemCRM.IsRunDev = false;
                                                itemCRM.WorkingDevelopers = itemEMS.WorkingDevelopers;
                                                itemCRM.Modifiydate = itemEMS.Modifiydate;
                                                itemCRM.Virtual_DevId = virtual_devID;
                                                itemCRM.Actual_DevId = actual_devID;
                                                itemCRM.Bill_Team = Bteam;

                                                model.ProjectsListInfo.Add(itemCRM);
                                            }
                                        }

                                        if (itemCRM.other_contact.ToLower() == "no" && !model.ProjectsListInfo.Any(x => x.Project_Id == itemCRM.Project_Id))
                                        {
                                            if (itemEMS.PMUId != pmuid)
                                            {
                                                itemCRM.IsNew = false;
                                                itemCRM.IsRunDev = false;
                                                itemCRM.WorkingDevelopers = itemEMS.WorkingDevelopers;
                                                itemCRM.Modifiydate = itemEMS.Modifiydate;
                                                itemCRM.Virtual_DevId = virtual_devID;
                                                itemCRM.Actual_DevId = actual_devID;
                                                itemCRM.Bill_Team = Bteam;
                                                model.ProjectsListInfo.Add(itemCRM);
                                            }
                                        }
                                    }


                                }
                                catch { }
                            }
                        }

                        foreach (var itemCRM in CRMProjectList)
                        {
                            if (EMSProjectList != null)
                            {
                                if (EMSProjectList.Select(X => X.EmsCRMPId.ToString()).Contains(itemCRM.Project_Id) == false && (itemCRM.Project_Status == "Running" || itemCRM.Project_Status == "Runing" || itemCRM.Project_Status == "OVER-RUN"))// && Convert.ToInt32(itemCRM.developer) !=0
                                {
                                    if (Convert.ToInt32(itemCRM.Developer) == 0 && (itemCRM.Project_Status == "Runing" || itemCRM.Project_Status == "Running"))
                                    {
                                        //cstatus = "On Hold";
                                        itemCRM.Project_Status = "On Hold";
                                        //itemCRM.C_Status = true;
                                    }

                                    string actual_dev_email = string.Empty;
                                    string virtual_dev_email = string.Empty;

                                    string actual_devID = string.Empty;
                                    string virtual_devID = string.Empty;

                                    if (itemCRM.Dev_Detail != null && itemCRM.Dev_Detail.Count > 0)
                                    {
                                        for (int i = 0; i <= itemCRM.Dev_Detail.Count - 1; i++)
                                        {
                                            actual_dev_email = string.Empty;

                                            if (itemCRM.Dev_Detail[i].Actual != null)
                                                actual_dev_email = itemCRM.Dev_Detail[i].Actual.Email.ToString();

                                            virtual_dev_email = string.Empty;

                                            if (itemCRM.Dev_Detail[i].Virtual_Dev != null)
                                                virtual_dev_email = itemCRM.Dev_Detail[i].Virtual_Dev.Email.ToString();

                                            if (userlist != null && virtual_devList != null)
                                            {
                                                if (userlist.Select(X => X.EmailOffice.ToString()).Contains(actual_dev_email) == true && !string.IsNullOrEmpty(actual_dev_email))
                                                {
                                                    var acdevid = userlist.FirstOrDefault(R => R.EmailOffice == Convert.ToString(actual_dev_email.ToLower().Trim()) && R.IsActive == true);
                                                    actual_devID += (acdevid == null ? "" : acdevid.Uid.ToString() + ",");
                                                }
                                                if (virtual_devList.Select(X => X.emailid.ToString()).Contains(virtual_dev_email) == true && !String.IsNullOrEmpty(virtual_dev_email))
                                                {
                                                    var vdevlist = virtual_devList.FirstOrDefault(a => a.emailid == Convert.ToString(virtual_dev_email.ToLower().Trim()) && a.isactive == true);
                                                    //var vdevlist = VirtualDeveloper.GetUsersbyemail(Convert.ToString(virtual_dev_email.ToLower().Trim()), true);
                                                    if (vdevlist != null)
                                                    {
                                                        virtual_devID += vdevlist.VirtualDeveloper_ID.ToString() + ",";
                                                    }
                                                }
                                                //virtual_devID += VirtualDeveloper.GetUsersbyemail(Convert.ToString(virtual_dev_email.ToLower().Trim()), true).VirtualDeveloper_ID.ToString() + ",";
                                            }
                                        }
                                    }

                                    itemCRM.IsNew = true;
                                    itemCRM.IsRunDev = false;

                                    itemCRM.Virtual_DevId = virtual_devID;
                                    itemCRM.Actual_DevId = actual_devID;
                                    model.ProjectsListInfo.Add(itemCRM);
                                }
                            }
                        }


                        //Get List of Removed other PM list 
                        var GetRemovedOtherPm = EMSProjectList.Where(x => !CRMProjectList.Select(s => s.Project_Id).ToList().Contains(x.EmsCRMPId) && x.ProjectOtherContactPMList.Any(pot => pot.PMUid == pmuid)).ToList();

                        foreach (var item in GetRemovedOtherPm)
                        {
                            // item.Project_Id = item.EmsCRMPId;
                            item.IsRemovedOtherPm = true;
                            model.ProjectsListInfo.Add(item);
                        }


                        if (model.ProjectsListInfo != null && model.ProjectsListInfo.Any())
                        {
                            foreach (var item in model.ProjectsListInfo)
                            {
                                if (item.IsRemovedOtherPm)
                                {
                                    item.CommandName = "Delete";
                                }
                                else
                                {
                                    item.CommandName = item.IsNew ? "add" : "update";
                                }

                                item.Project_Status = Common.GetBillingDisplayTeam(item.Project_Status);
                                //item.OtherContact = "yes";
                                //item.OtherContact = "no";
                            }
                            model.ProjectsListInfo = model.ProjectsListInfo.ToList();




                            return View(model);
                        }
                    }
                    else { ShowErrorMessage("Error", "No Data Recieved from Api", false); }
                }
                ShowErrorMessage("Error", "No Data Recieved from Api", false);

            }
            catch (Exception ex)
            {

                ShowErrorMessage("Error", ex.Message, true);
            }

            return View();
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult Index(ProjectsUpdateDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Guid TransId = Guid.NewGuid();
                    List<BucketModel> bucketModelData = bucketModelService.GetData();
                    //BucketModel.GetData()
                    var GetTech = technologyService.GetTechnologyList();
                    //Technology.GetTechnologies();

                    if (model.ProjectsListInfo != null && model.ProjectsListInfo.Any())
                    {
                        foreach (var project in model.ProjectsListInfo.Where(x => x.RecordCheckbox == true))
                        {
                            if (project.CommandName == "update")
                            {
                                Project projectEntity = null;
                                if (model.PmUid > 0)
                                {
                                    //projectEntity = projectService.GetProjectByCRMId(Convert.ToInt32(project.Project_Id), model.PmUid);
                                    projectEntity = projectService.GetProjectByCRMId(Convert.ToInt32(project.Project_Id));

                                    var dd = projectService.GetProjectById(projectEntity.ProjectId);

                                    if (projectEntity != null && !projectEntity.IsInHouse)
                                    {

                                        List<DeveloperDetailIDs> DeveloperDetails = new List<DeveloperDetailIDs>();
                                        if (!string.IsNullOrEmpty(project.Pdev_Detail))
                                        {
                                            project.Pdev_Detail.Split(',').ToList().ForEach(a =>
                                            {
                                                if (a.Split(':').Length == 2 && (a.Split(':')[1] != "0" || a.Split(':')[1] != ""))
                                                {
                                                    DeveloperDetails.Add(new DeveloperDetailIDs
                                                    {
                                                        ActualDeveloperID = a.Split(':')[0] == "0" ? null : Convert.ToInt32(a.Split(':')[0]) as Nullable<Int32>,
                                                        VirtualDeveloperID = Convert.ToInt32(a.Split(':')[1])
                                                    });
                                                }
                                            });
                                        }
                                        BucketModel bucktobj = bucketModelData.FirstOrDefault(P => P.ModelName.ToLower().Trim() == project.ModelName?.ToLower().Trim());
                                        if (bucktobj != null)
                                        {
                                            projectEntity.Model = bucktobj.BucketId;
                                            projectEntity.Status = Core.Common.GetEMSProjectStatus(project.Project_Status.ToLower().Trim());
                                            projectEntity.ActualDevelopers = Convert.ToInt32(project.Developer);
                                            projectEntity.BillingTeam = project.Bill_Team;
                                            projectEntity.StartDate = Convert.ToDateTime(project.Start_Date);
                                            projectEntity.ModifyDate = DateTime.Now;

                                            //projectEntity.Uid = CurrentUser.Uid;
                                            //projectEntity.Uid = SiteSession.SessionUser.Uid;



                                            projectEntity.AbroadPMUid = null;

                                            if (!string.IsNullOrWhiteSpace(project.Pm_Email))
                                            {
                                                var abroadPmInfo = userLoginService.GetLoginDeatilByEmail(project.Pm_Email);
                                                if (abroadPmInfo != null)
                                                {
                                                    projectEntity.AbroadPMUid = abroadPmInfo.Uid;
                                                }
                                            }

                                            if (projectEntity.ClientId == null && project.Client_Name != "")
                                            {

                                                Client Clientobj = new Client();
                                                {
                                                    Clientobj.Name = project.Client_Name;
                                                    Clientobj.Email = "";
                                                    Clientobj.Msn = "";
                                                    Clientobj.Phone = "";
                                                    Clientobj.IsActive = true;
                                                    Clientobj.Address = "";
                                                    Clientobj.ModifyDate = DateTime.Now;
                                                    Clientobj.AddDate = DateTime.Now;
                                                    Clientobj.Ip = GeneralMethods.Getip();


                                                    if (project.other_contact.ToLower() == "no")
                                                    {
                                                        Clientobj.PMUid = model.PmUid;
                                                    }


                                                    projectService.SaveClient(Clientobj);
                                                };

                                                projectEntity.ClientId = Clientobj.ClientId;
                                            }
                                            else if (projectEntity.Client != null)
                                            {
                                                projectEntity.Client.Name = project.Client_Name;
                                            }

                                            var project_techlist = projectService.GetDataProjectTechByProjectId(projectEntity.ProjectId);
                                            //Project_Tech.GetDataByProjectId(projectEntity.ProjectId)

                                            if (project_techlist.Count == 0)
                                            {
                                                string GetId = string.Empty;
                                                string DepartmentId = string.Empty;

                                                string[] Tech_arr = new string[] { };

                                                Tech_arr = (!string.IsNullOrEmpty(project.Primary_Technology) ? project.Primary_Technology.ToLower().Trim().Split(',') : null);

                                                if (Tech_arr != null && Tech_arr.Count() > 0)
                                                {
                                                    Tech_arr = Tech_arr.Select(l => string.Join(" ", l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))).ToArray();
                                                    string[] distinctArray = RemoveDuplicates(Tech_arr);

                                                    foreach (var TectList in GetTech)
                                                    {
                                                        if (TectList != null)
                                                        {
                                                            var Techresults = Array.FindAll(distinctArray, S => S.Equals(TectList.Title.ToLower().Trim()) || (TectList.Title.ToLower().Trim()).Contains(S));
                                                            if (Techresults.Length > 0)
                                                            {

                                                                string Techname = Techresults[0].ToString();
                                                                var technology = technologyService.GetTechnologyByName(Techname.Trim());
                                                                GetId += technology.TechId + ",";
                                                                //Technology.GetTechnologiesNameById(Techname).TechId + ",";

                                                                if (Techname.Contains("php") || Techname.Contains("wordPress") || Techname.Contains("joomla") || Techname.Contains("codeigniter") || Techname.Contains("drupal"))
                                                                {
                                                                    if (!DepartmentId.Contains(Enums.ProjectDepartment.PHPDevelopment.ToString()))
                                                                        DepartmentId += (int)Enums.ProjectDepartment.PHPDevelopment + ",";

                                                                }
                                                                else if (Techname.Contains("asp") || Techname.Contains("visual") || Techname.Contains("mvc") || Techname.Contains("linq") || Techname.Contains("entity") || Techname.Contains("dotnet nuke") || Techname.Contains("wcf") || Techname.Contains("sliverlight"))
                                                                {
                                                                    if (!DepartmentId.Contains(Enums.ProjectDepartment.DotNetDevelopment.ToString()))
                                                                        DepartmentId += (int)Enums.ProjectDepartment.DotNetDevelopment + ",";

                                                                }
                                                                else if (Techname.Contains("iphone") || Techname.Contains("andriod") || Techname.Contains("windows phone") || Techname.Contains("blackberry"))
                                                                {
                                                                    if (!DepartmentId.Contains(Enums.ProjectDepartment.MobileApplication.ToString()))
                                                                        DepartmentId += (int)Enums.ProjectDepartment.MobileApplication + ",";
                                                                }
                                                                else if (Techname.Contains("communication skill"))
                                                                {
                                                                    if (!DepartmentId.Contains(Enums.ProjectDepartment.BusinessAnalyst.ToString()))
                                                                        DepartmentId += (int)Enums.ProjectDepartment.BusinessAnalyst + ",";
                                                                }
                                                            }
                                                        }
                                                    }
                                                    string[] List_Technology = (!string.IsNullOrEmpty(GetId) ? GetId.TrimEnd(',').Split(',') : null);
                                                    string[] ldistinctArray = Tech_RemoveDuplicates(List_Technology);
                                                    if (ldistinctArray != null && ldistinctArray.Count() > 0)
                                                    {
                                                        for (int id = 0; id <= ldistinctArray.ToList().Count - 1; id++)
                                                        {
                                                            Project_Tech projecttechobj = new Project_Tech();
                                                            projecttechobj.ProjectID = projectEntity.ProjectId; ;
                                                            projecttechobj.TechId = Convert.ToInt32(ldistinctArray[id]);
                                                            projectService.SaveProjectTech(projecttechobj);
                                                            //projecttechobj.Save();
                                                        }

                                                    }
                                                    var Department_List = projectService.GetProject_DepartmentDataByProjectId(projectEntity.ProjectId);
                                                    /*Project_Department.GetDataByProjectId(projectEntity.ProjectId);*/
                                                    if (Department_List.Count == 0)
                                                    {

                                                        string[] project_GId = (!string.IsNullOrEmpty(DepartmentId) ? DepartmentId.TrimEnd(',').Split(',') : null);
                                                        string[] pdistinctArray = Department_RemoveDuplicates(project_GId);
                                                        if (pdistinctArray != null && pdistinctArray.Count() > 0)
                                                        {
                                                            for (int GID = 0; GID <= pdistinctArray.ToList().Count - 1; GID++)
                                                            {
                                                                //(new Project_Department
                                                                //{
                                                                //    ProjectID = projectEntity.ProjectId,
                                                                //    DeptID = Convert.ToInt32(pdistinctArray[GID])
                                                                //}).Save();

                                                                var data = new Project_Department
                                                                {
                                                                    ProjectID = projectEntity.ProjectId,
                                                                    DeptID = Convert.ToInt32(pdistinctArray[GID])
                                                                };
                                                                projectService.SaveProjectDept(data);
                                                            }

                                                            //(new Project_Department
                                                            //{
                                                            //    ProjectID = projectEntity.ProjectId,
                                                            //    DeptID = (int)Enums.ProjectDepartment.WebDesigning
                                                            //}).Save();
                                                            //(new Project_Department
                                                            //{
                                                            //    ProjectID = projectEntity.ProjectId,
                                                            //    DeptID = (int)Enums.ProjectDepartment.QualityAnalyst
                                                            //}).Save();

                                                            var data1 = new Project_Department
                                                            {
                                                                ProjectID = projectEntity.ProjectId,
                                                                DeptID = (int)Enums.ProjectDepartment.WebDesigning
                                                            };

                                                            var data2 = new Project_Department
                                                            {
                                                                ProjectID = projectEntity.ProjectId,
                                                                DeptID = (int)Enums.ProjectDepartment.WebDesigning
                                                            };

                                                            projectService.SaveProjectDept(data1);
                                                            projectService.SaveProjectDept(data2);

                                                        }
                                                    }
                                                }
                                            }

                                            if (projectEntity.ProjectDevelopers != null && DeveloperDetails.Any())
                                            {
                                                var projectdevList = projectEntity.ProjectDevelopers.ToList().FindAll(X => X.ProjectId == projectEntity.ProjectId);

                                                foreach (ProjectDeveloper pd in projectdevList)
                                                {
                                                    pd.WorkStatus = (int)Enums.ProjectDevWorkStatus.Closed;
                                                }

                                                foreach (var dev in DeveloperDetails)
                                                {
                                                    if (dev.VirtualDeveloperID != 0)
                                                    {
                                                        var projectDev = projectdevList.FirstOrDefault(x => x.VD_id == dev.VirtualDeveloperID);
                                                        if (projectDev != null)
                                                        {
                                                            projectDev.Uid = dev.ActualDeveloperID;
                                                            projectDev.WorkStatus = (int)Enums.ProjectDevWorkStatus.Running;
                                                            projectDev.ModifyDate = DateTime.Now;
                                                        }
                                                        else
                                                        {
                                                            var projctDeveloper = new ProjectDeveloper
                                                            {
                                                                ProjectId = Convert.ToInt32(projectEntity.ProjectId),
                                                                Uid = dev.ActualDeveloperID,
                                                                TransId = TransId,
                                                                Remark = "From CRM WebServics",
                                                                WorkStatus = (int)Enums.ProjectDevWorkStatus.Running,
                                                                //WorkRole = (int)Enums.ProjectDevWorkRole.Paid,
                                                                VD_id = dev.VirtualDeveloperID,
                                                                AddDate = DateTime.Now,
                                                                ModifyDate = DateTime.Now,
                                                                IP = GeneralMethods.Getip()
                                                            };

                                                            projectService.SaveProjectDeveloper(projctDeveloper);
                                                        }
                                                    }
                                                }
                                            }

                                            if (projectEntity.Status == "H" || projectEntity.Status == "C" || projectEntity.Status == "D" || projectEntity.Status == "N")
                                            {
                                                if (projectEntity.ProjectDevelopers != null && projectEntity.ProjectDevelopers.Any())
                                                {
                                                    projectEntity.ProjectDevelopers.ToList().ForEach(PD => PD.WorkStatus = (int)Enums.ProjectDevWorkStatus.Closed);
                                                }
                                            }


                                            // Update PMUID in main project table, if other_contact="no" comming 
                                            //from CRM API and remove this PMUID from ProjectOtherPM table if exists.
                                            if (project.other_contact.ToLower() == "no")
                                            {
                                                projectEntity.PMUid = model.PmUid;

                                                DeleteOtherPm(model.PmUid, projectEntity.ProjectId);
                                            }

                                            // Update PMUID in ProjectOtherPM table if other_contact="yes" coming from CRM API
                                            if (project.other_contact.ToLower() == "yes")
                                            {
                                                if (!projectEntity.ProjectOtherPm.Any(x => x.ProjectId == projectEntity.ProjectId && x.Pmuid == model.PmUid))
                                                {
                                                    projectEntity.ProjectOtherPm.Add(
                                                       new ProjectOtherPm
                                                       {
                                                           Pmuid = model.PmUid
                                                       });
                                                    //  projectService.SaveProjectEntity(projectEntity);
                                                }
                                            }

                                            projectService.SaveProjectEntity(projectEntity);
                                        }
                                        else
                                        {
                                            return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Model does't define in crm so record will not be updated.", IsSuccess = false });
                                        }




                                    }


                                }
                            }


                            else if (project.CommandName.Trim() == "add")
                            {
                                List<DeveloperDetailIDs> DeveloperDetails = new List<DeveloperDetailIDs>();

                                if (!string.IsNullOrEmpty(project.Pdev_Detail))
                                {
                                    project.Pdev_Detail.Split(',').ToList().ForEach(a =>
                                    {

                                        if (a.Split(':').Length == 2 && (a.Split(':')[1] != "0" || a.Split(':')[1] != ""))
                                        {
                                            DeveloperDetails.Add(new DeveloperDetailIDs
                                            {
                                                ActualDeveloperID = a.Split(':')[0] == "0" ? null : Convert.ToInt32(a.Split(':')[0]) as Nullable<Int32>,
                                                VirtualDeveloperID = Convert.ToInt32(a.Split(':')[1])
                                            });
                                        }
                                    });
                                }

                                Project projectobj = new Project();
                                {
                                    projectobj.Name = project.Project_Name;
                                    projectobj.AddDate = DateTime.Now;
                                    projectobj.ModifyDate = DateTime.Now;
                                    projectobj.CRMProjectId = Convert.ToInt32(project.Project_Id);
                                    projectobj.BillingTeam = project.Bill_Team;
                                    projectobj.EstimateTime = Convert.ToInt32(project.Estimate_Time);
                                    projectobj.StartDate = Convert.ToDateTime(project.Start_Date);
                                    //projectobj.Uid = SiteSession.SessionUser.Uid;

                                    if (project.Pm_Email != null && project.Pm_Email != "")
                                    {
                                        var abroadPmInfo = userLoginService.GetLoginDeatilByEmail(project.Pm_Email);
                                        //UserLogin.GetForgotMailByEmailId(project.Pm_Email)
                                        if (abroadPmInfo != null)
                                        {
                                            projectobj.AbroadPMUid = abroadPmInfo.Uid;
                                        }
                                    }

                                    BucketModel bucktobj = bucketModelData.FirstOrDefault(P => P.ModelName.ToLower().Trim() == project.ModelName.ToLower().Trim());

                                    if (project.Client_Name != "")
                                    {
                                        Client Clientobj = new Client();
                                        {
                                            Clientobj.Name = project.Client_Name;
                                            Clientobj.Email = "";
                                            Clientobj.Msn = "";
                                            Clientobj.Phone = "";
                                            Clientobj.IsActive = true;
                                            Clientobj.Address = "";
                                            Clientobj.ModifyDate = DateTime.Now;
                                            Clientobj.AddDate = DateTime.Now;
                                            Clientobj.Ip = GeneralMethods.Getip();
                                            Clientobj.PMUid = model.PmUid;
                                            projectService.SaveClient(Clientobj);
                                            //Clientobj.Save();
                                        }
                                        projectobj.ClientId = Clientobj.ClientId;
                                    }

                                    if (bucktobj != null)
                                    {
                                        projectobj.Model = bucktobj.BucketId;
                                    }
                                    else
                                    {
                                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = $"Please add this bucket model:{project.ModelName}", IsSuccess = false });
                                    }
                                    if (Convert.ToInt32(project.Developer) == 0 && project.Project_Status == "Hold")
                                    {
                                        projectobj.ActualDevelopers = 0;
                                        projectobj.Status = "H";

                                    }
                                    else
                                    {

                                        projectobj.ActualDevelopers = Convert.ToInt32(project.Developer);
                                        projectobj.Status = Core.Common.GetEMSProjectStatus(project.Project_Status.ToLower().Trim());
                                    }
                                    projectobj.PMUid = model.PmUid;
                                    projectobj.IP = GeneralMethods.Getip();


                                    // for other PM
                                    if (project.other_contact.ToLower() == "yes")
                                    {
                                        projectobj.ProjectOtherPm.Add(
                                            new ProjectOtherPm
                                            {
                                                Pmuid = model.PmUid
                                            });
                                    }



                                    projectService.SaveProjectEntity(projectobj);
                                }



                                string GetId = string.Empty;
                                string DepartmentId = string.Empty;

                                string[] Tech_arr = new string[] { };

                                Tech_arr = (!String.IsNullOrEmpty(project.Primary_Technology) ? project.Primary_Technology.ToLower().Trim().Split(',') : null);

                                if (Tech_arr != null && Tech_arr.Count() > 0)
                                {
                                    Tech_arr = Tech_arr.Select(l => String.Join(" ", l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))).ToArray();
                                    string[] distinctArray = RemoveDuplicates(Tech_arr);

                                    foreach (var TectList in GetTech)
                                    {
                                        if (TectList != null)
                                        {
                                            var Techresults = Array.FindAll(distinctArray, S => S.Equals(TectList.Title.ToLower().Trim()) || (TectList.Title.ToLower().Trim()).Contains(S));
                                            if (Techresults != null && Techresults.Length > 0)
                                            {
                                                string Techname = Techresults[0].ToString();
                                                var technology = technologyService.GetTechnologyByName(Techname.Trim());
                                                GetId += technology.TechId + ",";
                                                //Technology.GetTechnologiesNameById(Techname)

                                                if (Techname.Contains("php") || Techname.Contains("wordPress") || Techname.Contains("joomla") || Techname.Contains("codeigniter") || Techname.Contains("drupal"))
                                                {
                                                    if (!DepartmentId.Contains(Enums.ProjectDepartment.PHPDevelopment.ToString()))
                                                        DepartmentId += (int)Enums.ProjectDepartment.PHPDevelopment + ",";

                                                }
                                                else if (Techname.Contains("asp") || Techname.Contains("visual") || Techname.Contains("mvc") || Techname.Contains("linq") || Techname.Contains("entity") || Techname.Contains("dotnet nuke") || Techname.Contains("wcf") || Techname.Contains("sliverlight"))
                                                {
                                                    if (!DepartmentId.Contains(Enums.ProjectDepartment.DotNetDevelopment.ToString()))
                                                        DepartmentId += (int)Enums.ProjectDepartment.DotNetDevelopment + ",";

                                                }
                                                else if (Techname.Contains("iphone") || Techname.Contains("andriod") || Techname.Contains("windows phone") || Techname.Contains("blackberry"))
                                                {
                                                    if (!DepartmentId.Contains(Enums.ProjectDepartment.MobileApplication.ToString()))
                                                        DepartmentId += (int)Enums.ProjectDepartment.MobileApplication + ",";
                                                }
                                                else if (Techname.Contains("communication skill"))
                                                {
                                                    if (!DepartmentId.Contains(Enums.ProjectDepartment.BusinessAnalyst.ToString()))
                                                        DepartmentId += (int)Enums.ProjectDepartment.BusinessAnalyst + ",";
                                                }
                                                //if (!DepartmentId.Contains(Enums.ProjectGroup.Designer.ToString()))
                                                //    GroupId += (int)Enums.ProjectGroup.Designer + ",";


                                            }
                                        }
                                    }


                                    string[] List_Technology = (!String.IsNullOrEmpty(GetId) ? GetId.TrimEnd(',').Split(',') : null);
                                    string[] ldistinctArray = Tech_RemoveDuplicates(List_Technology);
                                    if (ldistinctArray != null && ldistinctArray.Count() > 0)
                                    {
                                        for (int id = 0; id <= ldistinctArray.ToList().Count - 1; id++)
                                        {

                                            var projTech = new Project_Tech
                                            {
                                                ProjectID = projectobj.ProjectId,
                                                TechId = Convert.ToInt32(ldistinctArray[id])
                                            };

                                            projectService.SaveProjectTech(projTech);


                                        }

                                    }
                                    string[] project_GId = (!String.IsNullOrEmpty(DepartmentId) ? DepartmentId.TrimEnd(',').Split(',') : null);
                                    string[] pdistinctArray = Department_RemoveDuplicates(project_GId);
                                    if (pdistinctArray != null && pdistinctArray.Count() > 0)
                                    {
                                        for (int GID = 0; GID <= pdistinctArray.ToList().Count - 1; GID++)
                                        {
                                            var data = new Project_Department
                                            {
                                                ProjectID = projectobj.ProjectId,
                                                DeptID = Convert.ToInt32(pdistinctArray[GID])
                                            };
                                            projectService.SaveProjectDept(data);

                                        }
                                        var data1 = new Project_Department
                                        {
                                            ProjectID = projectobj.ProjectId,
                                            DeptID = (int)Enums.ProjectDepartment.WebDesigning
                                        };
                                        projectService.SaveProjectDept(data1);

                                    }
                                }
                                foreach (var dev in DeveloperDetails)
                                {
                                    if (dev.VirtualDeveloperID != 0)
                                    {

                                        var developer = new ProjectDeveloper
                                        {
                                            ProjectId = Convert.ToInt32(projectobj.ProjectId),
                                            Uid = dev.ActualDeveloperID,
                                            TransId = TransId,
                                            Remark = "From CRM WebServics",
                                            WorkStatus = (int)Enums.ProjectDevWorkStatus.Running,
                                            //WorkRole = (int)Enums.ProjectDevWorkRole.Paid,
                                            VD_id = dev.VirtualDeveloperID,
                                            AddDate = DateTime.Now,
                                            IP = GeneralMethods.Getip()
                                        };

                                        projectService.SaveProjectDeveloper(developer);

                                    }
                                }
                                if (projectobj.Status == "H" || projectobj.Status == "C" || projectobj.Status == "D")
                                {
                                    if (projectobj.ProjectDevelopers != null && projectobj.ProjectDevelopers.Count > 0)
                                    {
                                        projectobj.ProjectDevelopers.ToList().ForEach(PD => PD.WorkStatus = (int)Enums.ProjectDevWorkStatus.Closed);




                                        projectService.SaveProjectEntity(projectobj);
                                    }
                                }




                            }


                            else if (project.CommandName == "Delete")
                            {
                                if (model.PmUid > 0)
                                {
                                    // Remove PM from OtherProjectPm Table 
                                    DeleteOtherPm(model.PmUid, Convert.ToInt32(project.Project_Id));
                                }
                            }

                        }
                        ShowSuccessMessage("Success", "Record has been updated successfully", false);
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            RedirectUrl = Url.Action("Index", new { id = model.PmUidEncrypted })
                        });

                    }
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = $"No Record is there to update", IsSuccess = false });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Model does't define in crm so record will not be updated.", IsSuccess = false });
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string>
                {
                    ErrorMessage = ex.Message
                });
            }
        }


        public void DeleteOtherPm(int PmUid, int Project_Id)
        {

            var otherpm = projectService.GetProjectOtherPmsByPmuidAndProjectId(PmUid, Project_Id);
            if (otherpm.Count > 0)
                projectService.DeleteProjectOtherPms(otherpm);

        }

        private string[] RemoveDuplicates(string[] Tech_arr)
        {
            if (Tech_arr != null)
            {
                return Tech_arr.Distinct().ToArray<string>();
            }
            else { return null; }


        }

        private string[] Tech_RemoveDuplicates(string[] List_Technology)
        {
            if (List_Technology != null)
            {
                return List_Technology.Distinct().ToArray<string>();
            }
            else { return null; }
        }

        private string[] Group_RemoveDuplicates(string[] project_GId)
        {
            if (project_GId != null)
            {
                return project_GId.Distinct().ToArray<string>();
            }
            else { return null; }
        }

        private string[] Department_RemoveDuplicates(string[] project_DId)
        {
            if (project_DId != null)
            {
                return project_DId.Distinct().ToArray<string>();
            }
            else { return null; }
        }

    }
}
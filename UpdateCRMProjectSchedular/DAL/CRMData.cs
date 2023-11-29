using EMS.Core;
using EMS.Data;
using EMS.Data.Model;
using EMS.Dto;
using EMS.Service;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UpdateCRMProjectSchedular.Model;
using ProjectOtherContactPM = UpdateCRMProjectSchedular.Model.ProjectOtherContactPM;

namespace UpdateCRMProjectSchedular.DAL
{
    public static class CRMData
    {
        private static IProjectService projectService;
        private static IUserLoginService userLoginService;
        private static IVirtualDeveloperService virtualDeveloperService;
        private static IBucketModelService bucketModelService;
        private static ITechnologyService technologyService;
        public static void RegisterService(ServiceProvider _serviceProvider)
        {
            projectService = _serviceProvider.GetService<IProjectService>();
            userLoginService = _serviceProvider.GetService<IUserLoginService>();
            virtualDeveloperService = _serviceProvider.GetService<IVirtualDeveloperService>();
            bucketModelService = _serviceProvider.GetService<IBucketModelService>();
            technologyService = _serviceProvider.GetService<ITechnologyService>();
        }

        static List<UserJson> Lsproject = new List<UserJson>();
        public static void UpdateCRMData(int pmuid, UserLogin objUser, bool IsSuperAdmin)
        {
            try
            {
                bool IsLive = SiteKeys.IsLive;
                int CrmUserid = 0;
                string ApiPass = "";
                if (objUser != null)
                {
                    CrmUserid = objUser.CRMUserId != null ? objUser.CRMUserId.Value : 0;
                    ApiPass = String.IsNullOrEmpty(objUser.ApiPassword) ? "" : objUser.ApiPassword;
                }
                string jsonString = new WebClient().DownloadString(SiteKeys.CRMAPIOld + CrmUserid + "&apipass=" + ApiPass);
                jsonString = jsonString.Replace("\"virtual\":", "\"virtual_dev\":");
                jsonString = jsonString.Replace("\"dev_detail\":\"\"", "\"dev_detail\":[{}]");//\"virtual_dev\":{\"username\":\"\",\"email\":\"\"}
                JObject obj = JObject.Parse(jsonString);
                var token = (JArray)obj.SelectToken("response");
                String json = null;
                foreach (var item in token)
                {
                    json = JsonConvert.SerializeObject(item.SelectToken("result"));
                }
                if (json != null)
                {
                    List<UserJson> CRMProjectList = JsonConvert.DeserializeObject<List<UserJson>>(json).ToList();



                    if (CRMProjectList != null && CRMProjectList.Any())
                    {
                        #region "Get EMS Projects"
                        
                        var EMSProjects = projectService.GetProjectsCompByPMUid(pmuid);


                        var EMSProjectList = new List<UserJson>();

                        var pmUsers = userLoginService.GetUsersListByUserIds(EMSProjects.Where(x => x.AbroadPMUid.HasValue && x.AbroadPMUid.Value > 0)
                                                               .Select(x => x.AbroadPMUid.Value).Distinct().ToList());
                        foreach (var prj in EMSProjects)
                        {
                            EMSProjectList.Add(new UserJson()
                            {
                                project_id = Convert.ToString(prj.ProjectId),
                                EmsCRMPId = Convert.ToString(prj.CRMProjectId),
                                project_name = prj.Name,
                                pm_email = pmUsers.FirstOrDefault(u => u.Uid == (prj.AbroadPMUid ?? 0))?.EmailOffice ?? "",
                                project_status = prj.Status,
                                developer = Convert.ToString(prj.ActualDevelopers),
                                modelname = prj.BucketModel?.ModelName ?? "",
                                //WorkingDevelopers = prj.WorkingDevelopers,
                                bill_team = prj.BillingTeam,
                                estimate_time = Convert.ToString(prj.EstimateTime),
                                start_date = Convert.ToDateTime(prj.StartDate),
                                client_name = prj.Client?.Name ?? "",
                                projectDevelopers = prj.ProjectDevelopers != null ? prj.ProjectDevelopers.Select(p =>
                                new proj_devdetail
                                {
                                    // ProjectDeveloperID = p.ProjectDeveloperID,
                                    WorkStatus = p.WorkStatus,
                                    VD_id = p.VD_id,
                                    AD_id = p.Uid
                                }).ToList() : null,
                                PMUId = prj.PMUid,
                                ProjectOtherContactPMList = prj.ProjectOtherPm != null ? prj.ProjectOtherPm.Select(PO => new ProjectOtherContactPM { PMUid = PO.Pmuid, ProjectId = PO.ProjectId }).ToList() : new List<ProjectOtherContactPM>()

                            });
                        }

                        #endregion

                        List<UserLogin> userlist = userLoginService.GetUsersByPM(pmuid) ?? new List<UserLogin>();
                        List<VirtualDeveloper> virtualDeveloperList = virtualDeveloperService.GetVirtualDeveloperByPMUid(pmuid) ?? new List<VirtualDeveloper>();

                        var emsCRMProjectIds = EMSProjectList.Select(p => p.EmsCRMPId);

                        // Hard Coded Project CRM Id for Testing (in update case)
                       //  emsCRMProjectIds = EMSProjectList.Where(p => p.EmsCRMPId == "1161").Select(X => X.EmsCRMPId); 



                        var crmProjectIds = CRMProjectList.Select(x => x.project_id);

                        #region "Update Existing EMS Projects"

                        var CRMFilteredProjectList = CRMProjectList.Where(p => emsCRMProjectIds.Contains(p.project_id)).ToList();


                        


                        List<BucketModel> bucketModelDataList = bucketModelService.GetData();
                        var getTechList = technologyService.GetTechnologyList();
                        foreach (var itemCRM in CRMFilteredProjectList)
                        {
                            var itemEMS = EMSProjectList.FirstOrDefault(x => x.EmsCRMPId == itemCRM.project_id);

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

                                    var list = itemEMS.projectDevelopers.Where(p => p.WorkStatus == (int)Enums.ProjectDevWorkStatus.Running).ToList();
                                    if (list != null && list.Count > 0)
                                    {
                                        var ListId = list.Select(X => X.VD_id).ToList();

                                        for (int v = 0; v <= ListId.Count - 1; v++)
                                        {
                                            var vid = virtualDeveloperList.FirstOrDefault(a => a.VirtualDeveloper_ID == Convert.ToInt32(ListId[v]) && a.isactive == true);
                                            Emsvirtual_dev_ID += vid != null ? vid.VirtualDeveloper_ID.ToString() + "," : "";
                                        }

                                        var listADId = list.Select(X => X.AD_id).ToList();

                                        for (int v = 0; v <= listADId.Count - 1; v++)
                                        {
                                            var adid = userlist.FirstOrDefault(a => a.Uid == Convert.ToInt32(listADId[v]) && a.IsActive == true);
                                            Emsactual_dev_ID += adid != null ? adid.Uid.ToString() + "," : "";
                                        }
                                    }

                                    string strdevdetails = "";
                                    if (itemCRM.dev_detail != null && itemCRM.dev_detail.Count > 0)
                                    {
                                        for (int i = 0; i <= itemCRM.dev_detail.Count - 1; i++)
                                        {
                                            int actid = 0;
                                            int vdevid = 0;
                                            actual_dev_email = string.Empty;
                                            if (itemCRM.dev_detail[i].actual != null)
                                            {
                                                try
                                                {
                                                    actual_dev_email = (itemCRM.dev_detail[i].actual.email ?? "").ToLower().Trim();
                                                }
                                                catch
                                                {
                                                    actual_dev_email = "";
                                                }
                                            }

                                            virtual_dev_email = string.Empty;
                                            if (itemCRM.dev_detail[i].virtual_dev != null)
                                            {
                                                try
                                                {
                                                    virtual_dev_email = (itemCRM.dev_detail[i].virtual_dev.email ?? "").ToLower().Trim();
                                                }
                                                catch
                                                {
                                                    virtual_dev_email = "";
                                                }
                                            }

                                            if (userlist != null && virtualDeveloperList != null)
                                            {
                                                if (!String.IsNullOrEmpty(actual_dev_email) && userlist.Select(X => X.EmailOffice.ToLower()).Contains(actual_dev_email) == true)
                                                {
                                                    var acdevid = userlist.FirstOrDefault(R => R.EmailOffice.ToLower().Trim() == actual_dev_email && R.IsActive == true);
                                                    actual_devID += (acdevid != null ? (acdevid.Uid.ToString() + ",") : "");
                                                    actid = (acdevid != null ? acdevid.Uid : 0);
                                                }

                                                if (!String.IsNullOrEmpty(virtual_dev_email))
                                                {
                                                    Boolean isvDev = virtualDeveloperList.Any(a => a.emailid.ToLower().Trim() == virtual_dev_email && a.isactive == true);
                                                    if (isvDev == false)
                                                    {
                                                        // IF developer not exist in system then first we will add in system 
                                                        string vname = string.Empty;
                                                        try
                                                        {
                                                            vname = virtual_dev_email.Split('@')[0].Trim().Replace(".", " ");
                                                        }
                                                        catch { }

                                                        virtualDeveloperService.Save(new VirtualDeveloper
                                                        {
                                                            VirtualDeveloper_Name = vname,
                                                            SkypeId = "",
                                                            isactive = true,
                                                            emailid = virtual_dev_email,
                                                            Ismain = true,
                                                            PMUid = pmuid,
                                                            ModifiedDate = DateTime.Now,
                                                            CreationDate = DateTime.Now
                                                        });

                                                        virtualDeveloperList = virtualDeveloperService.GetVirtualDeveloperByPMUid(pmuid);
                                                    }
                                                    var vdevlist = virtualDeveloperList.FirstOrDefault(a => a.emailid.ToLower().Trim() == virtual_dev_email && a.isactive == true);
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
                                    itemCRM.pdev_detail = strdevdetails;
                                    string[] _emsVID = (!String.IsNullOrEmpty(Emsvirtual_dev_ID) ? Emsvirtual_dev_ID.TrimEnd(',').Split(',') : null);
                                    string[] _crmVID = (!String.IsNullOrEmpty(virtual_devID) ? virtual_devID.TrimEnd(',').Split(',') : null);

                                    int flag = 0;
                                    if (Convert.ToInt32(itemCRM.developer) == 0 && (itemCRM.project_status == "Runing" || itemCRM.project_status == "Running"))
                                    {
                                        itemCRM.project_status = "On Hold";
                                    }

                                    string cstatus = GetEMSProjectStatus(itemCRM.project_status.ToLower().Trim());
                                    string cstatusdetail = itemCRM.project_status.Trim();

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
                                        if (!((itemEMS.project_status.ToUpper() == "C" || itemEMS.project_status.ToUpper() == "H" || itemEMS.project_status.ToUpper() == "D" || itemEMS.project_status.ToUpper() == "N") &&
                                        itemEMS.project_status.ToUpper() == cstatus.ToUpper()))
                                        {
                                            flag++;
                                        }
                                    }

                                    if (flag == 0)
                                    {
                                        string[] _emsADID = (!String.IsNullOrEmpty(Emsactual_dev_ID) ? Emsactual_dev_ID.TrimEnd(',').Split(',') : null);
                                        string[] _crmADID = (!String.IsNullOrEmpty(actual_devID) ? actual_devID.TrimEnd(',').Split(',') : null);

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
                                            if (!((itemEMS.project_status.ToUpper() == "C" || itemEMS.project_status.ToUpper() == "H" || itemEMS.project_status.ToUpper() == "D" || itemEMS.project_status.ToUpper() == "N") &&
                                            itemEMS.project_status.ToUpper() == cstatus.ToUpper()))
                                            {
                                                flag++;
                                            }
                                        }
                                    }

                                    string crmPMEmail = string.IsNullOrEmpty(itemCRM.pm_email) ? "" : Convert.ToString(itemCRM.pm_email);

                                    string Bteam = GetBillingTeam(itemCRM.bill_team.ToLower().Trim());
                                    if (flag > 0 || itemCRM.developer != Convert.ToString(itemEMS.developer) || itemCRM.modelname.ToLower().Trim() != itemEMS.modelname.ToLower().Trim() ||
                                        cstatus.ToLower().Trim() != itemEMS.project_status.ToLower().Trim() || Bteam.ToLower().Trim() != itemEMS.bill_team.ToLower().Trim() ||
                                        itemCRM.start_date != itemEMS.start_date || (itemEMS.client_name == "" && itemCRM.client_name != "")
                                        || ((itemEMS.pm_email != crmPMEmail )
                                        && (itemCRM.modelname.ToLower().Trim() == "blended -pm from uk office" ||
                                        itemCRM.modelname.ToLower().Trim() == "bucket system- blended" ||
                                        itemCRM.modelname.ToLower().Trim() == "uk pm support" || itemCRM.modelname.ToLower().Trim() == "bucket system- blended uk")))
                                        //&& cstatus == "R" &&
                                    {
                                        itemCRM.IsNew = false;
                                        itemCRM.IsRunDev = false;
                                        itemCRM.WorkingDevelopers = itemEMS.WorkingDevelopers;
                                        itemCRM.Modifiydate = itemEMS.Modifiydate;
                                        itemCRM.virtual_devId = virtual_devID;
                                        itemCRM.actual_devId = actual_devID;
                                        //var billteam = Convert.ToString(itemCRM.bill_team) == "India" ? "INR" : Convert.ToString(itemCRM.bill_team) == "India FL" ? "INRFL" : Convert.ToString(itemCRM.bill_team) == "UK" ? "UK" : Convert.ToString(itemCRM.bill_team) == "US" || Convert.ToString(itemCRM.bill_team) == "USA" ? "USA" : Convert.ToString(itemCRM.bill_team) == "Australia" || Convert.ToString(itemCRM.bill_team) == "AUS" ? "AUS" : " ";
                                        //var status = Convert.ToString(itemCRM.project_status) == "H" || Convert.ToString(itemCRM.project_status) == "On Hold" ? "Hold" : Convert.ToString(itemCRM.project_status) == "R" || Convert.ToString(itemCRM.project_status) == "Runing" ? "Running" : Convert.ToString(itemCRM.project_status) == "D" || Convert.ToString(itemCRM.project_status) == "Remove" ? "Deactive" : Convert.ToString(itemCRM.project_status) == "C" || Convert.ToString(itemCRM.project_status) == "Complete" ? "Completed" : Convert.ToString(itemCRM.project_status) == "I" || Convert.ToString(itemCRM.project_status) == "Not Initiated" ? "Not Initiated" : Convert.ToString(itemCRM.project_status) == "OVER-RUN" ? "Over Run" : "-";
                                        UpdateData(pmuid, pmuid, itemCRM.project_id, Bteam, itemCRM.estimate_time, itemCRM.start_date.ToString(), itemCRM.primary_technology, itemCRM.client_name, itemCRM.project_name, itemCRM.modelname, itemCRM.developer,
                                           itemCRM.project_id, cstatus, itemCRM.pdev_detail, crmPMEmail, bucketModelDataList, getTechList, cstatusdetail, IsSuperAdmin, itemCRM.other_contact);
                                    }
                                    else if (itemCRM.developer != Convert.ToString(itemEMS.WorkingDevelopers) && itemEMS.WorkingDevelopers > 0)
                                    {
                                        itemCRM.IsRunDev = true;
                                        itemCRM.IsNew = false;
                                        itemCRM.WorkingDevelopers = itemEMS.WorkingDevelopers;
                                        itemCRM.Modifiydate = itemEMS.Modifiydate;
                                        itemCRM.virtual_devId = virtual_devID;
                                        itemCRM.actual_devId = actual_devID;
                                        //var billteam = Convert.ToString(itemCRM.bill_team) == "India" ? "INR" : Convert.ToString(itemCRM.bill_team) == "India FL" ? "INRFL" : Convert.ToString(itemCRM.bill_team) == "UK" ? "UK" : Convert.ToString(itemCRM.bill_team) == "US" || Convert.ToString(itemCRM.bill_team) == "USA" ? "USA" : Convert.ToString(itemCRM.bill_team) == "Australia" || Convert.ToString(itemCRM.bill_team) == "AUS" ? "AUS" : " ";
                                        //var status = Convert.ToString(itemCRM.project_status) == "H" || Convert.ToString(itemCRM.project_status) == "On Hold" ? "Hold" : Convert.ToString(itemCRM.project_status) == "R" || Convert.ToString(itemCRM.project_status) == "Runing" ? "Running" : Convert.ToString(itemCRM.project_status) == "D" || Convert.ToString(itemCRM.project_status) == "Remove" ? "Deactive" : Convert.ToString(itemCRM.project_status) == "C" || Convert.ToString(itemCRM.project_status) == "Complete" ? "Completed" : Convert.ToString(itemCRM.project_status) == "I" || Convert.ToString(itemCRM.project_status) == "Not Initiated" ? "Not Initiated" : Convert.ToString(itemCRM.project_status) == "OVER-RUN" ? "Over Run" : "-";
                                        UpdateData(pmuid, pmuid, itemCRM.project_id, Bteam, itemCRM.estimate_time, itemCRM.start_date.ToString(), itemCRM.primary_technology, itemCRM.client_name, itemCRM.project_name, itemCRM.modelname, itemCRM.developer,
                                           itemCRM.project_id, cstatus, itemCRM.pdev_detail, crmPMEmail, bucketModelDataList, getTechList, cstatusdetail, IsSuperAdmin, itemCRM.other_contact);
                                    }

                                    if (itemCRM.other_contact.ToLower() == "yes")
                                    {
                                        //if (itemEMS.PMUId == pmuid)
                                        if (!itemEMS.ProjectOtherContactPMList.Any(p => p.PMUid == pmuid))
                                        {
                                            itemCRM.IsNew = false;
                                            itemCRM.IsRunDev = false;
                                            itemCRM.WorkingDevelopers = itemEMS.WorkingDevelopers;
                                            itemCRM.Modifiydate = itemEMS.Modifiydate;
                                            itemCRM.virtual_devId = virtual_devID;
                                            itemCRM.actual_devId = actual_devID;
                                            //itemCRM.bill_team = Bteam;

                                            UpdateData(pmuid, pmuid, itemCRM.project_id, Bteam, itemCRM.estimate_time, itemCRM.start_date.ToString(), itemCRM.primary_technology, itemCRM.client_name, itemCRM.project_name, itemCRM.modelname, itemCRM.developer,
                                           itemCRM.project_id, cstatus, itemCRM.pdev_detail, crmPMEmail, bucketModelDataList, getTechList, cstatusdetail, IsSuperAdmin, itemCRM.other_contact);

                                        }
                                    }

                                    if (itemCRM.other_contact.ToLower() == "no")
                                    {
                                        if (itemEMS.PMUId != pmuid)
                                        {
                                            itemCRM.IsNew = false;
                                            itemCRM.IsRunDev = false;
                                            itemCRM.WorkingDevelopers = itemEMS.WorkingDevelopers;
                                            itemCRM.Modifiydate = itemEMS.Modifiydate;
                                            itemCRM.virtual_devId = virtual_devID;
                                            itemCRM.actual_devId = actual_devID;
                                            itemCRM.bill_team = Bteam;
                                            UpdateData(pmuid, pmuid, itemCRM.project_id, Bteam, itemCRM.estimate_time, itemCRM.start_date.ToString(), itemCRM.primary_technology, itemCRM.client_name, itemCRM.project_name, itemCRM.modelname, itemCRM.developer,
                                            itemCRM.project_id, cstatus, itemCRM.pdev_detail, crmPMEmail, bucketModelDataList, getTechList, cstatusdetail, IsSuperAdmin, itemCRM.other_contact);

                                        }
                                    }


                                }
                                catch (Exception ex)
                                {
                                    ManageLog.WriteLogFile("============================== Update Data (Main Method) for Project Id (" + itemCRM.project_id + ") : Error (" + DateTime.Now.ToString() + ") ==============================");
                                    ManageLog.WriteLogFile((ex.InnerException ?? ex).Message);
                                    ManageLog.WriteLogFile((ex.InnerException ?? ex).StackTrace);
                                    ManageLog.WriteLogFile("============================== /Error ==============================");
                                }
                            }
                        }

                       
                        #endregion





                        #region "Add New CRM Projects in EMS"

                        foreach (var itemCRM in CRMProjectList)
                        {
                            if (EMSProjectList.Select(X => X.EmsCRMPId).Contains(itemCRM.project_id) == false && (itemCRM.project_status == "Running" || itemCRM.project_status == "Runing" || itemCRM.project_status == "OVER-RUN"))// && Convert.ToInt32(itemCRM.developer) !=0
                            {
                                try
                                {
                                    if (Convert.ToInt32(itemCRM.developer) == 0 && (itemCRM.project_status == "Runing" || itemCRM.project_status == "Running"))
                                    {
                                        itemCRM.project_status = "On Hold";
                                    }

                                    string actual_dev_email = string.Empty;
                                    string virtual_dev_email = string.Empty;

                                    string actual_devID = string.Empty;
                                    string virtual_devID = string.Empty;

                                    if (itemCRM.dev_detail != null && itemCRM.dev_detail.Any())
                                    {
                                        for (int i = 0; i <= itemCRM.dev_detail.Count - 1; i++)
                                        {
                                            actual_dev_email = string.Empty;

                                            if (itemCRM.dev_detail[i].actual != null)
                                                actual_dev_email = itemCRM.dev_detail[i].actual.email.ToString();

                                            virtual_dev_email = string.Empty;

                                            if (itemCRM.dev_detail[i].virtual_dev != null)
                                                virtual_dev_email = itemCRM.dev_detail[i].virtual_dev.email.ToString();

                                            if (userlist != null && virtualDeveloperList != null)
                                            {
                                                if (userlist.Select(X => X.EmailOffice.ToString().ToLower()).Contains(actual_dev_email.ToLower()) == true && !String.IsNullOrEmpty(actual_dev_email))
                                                {
                                                    var acdevid = userlist.FirstOrDefault(R => R.EmailOffice.ToLower().Trim() == Convert.ToString(actual_dev_email.ToLower().Trim()) && R.IsActive == true);
                                                    actual_devID += (acdevid == null ? "" : acdevid.Uid.ToString() + ",");
                                                }
                                                if (virtualDeveloperList.Select(X => X.emailid.ToString().ToLower()).Contains(virtual_dev_email.ToLower()) == true && !String.IsNullOrEmpty(virtual_dev_email))
                                                {
                                                    var vdevlist = virtualDeveloperList.FirstOrDefault(a => a.emailid.ToLower().Trim() == Convert.ToString(virtual_dev_email.ToLower().Trim()) && a.isactive == true);
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

                                    itemCRM.virtual_devId = virtual_devID;
                                    itemCRM.actual_devId = actual_devID;

                                    string crmPMEmail = string.IsNullOrEmpty(itemCRM.pm_email) ? "" : Convert.ToString(itemCRM.pm_email);

                                    //var billteam = Convert.ToString(itemCRM.bill_team) == "India" ? "INR" : Convert.ToString(itemCRM.bill_team) == "India FL" ? "INRFL" : Convert.ToString(itemCRM.bill_team) == "UK" ? "UK" : Convert.ToString(itemCRM.bill_team) == "US" || Convert.ToString(itemCRM.bill_team) == "USA" ? "USA" : Convert.ToString(itemCRM.bill_team) == "Australia" || Convert.ToString(itemCRM.bill_team) == "AUS" ? "AUS" : " ";
                                    //var status = Convert.ToString(itemCRM.project_status) == "H" || Convert.ToString(itemCRM.project_status) == "On Hold" ? "Hold" : Convert.ToString(itemCRM.project_status) == "R" || Convert.ToString(itemCRM.project_status) == "Runing" ? "Running" : Convert.ToString(itemCRM.project_status) == "D" || Convert.ToString(itemCRM.project_status) == "Remove" ? "Deactive" : Convert.ToString(itemCRM.project_status) == "C" || Convert.ToString(itemCRM.project_status) == "Complete" ? "Completed" : Convert.ToString(itemCRM.project_status) == "I" || Convert.ToString(itemCRM.project_status) == "Not Initiated" ? "Not Initiated" : Convert.ToString(itemCRM.project_status) == "OVER-RUN" ? "Over Run" : "-";
                                    string billteam = GetBillingTeam(itemCRM.bill_team.ToLower().Trim());
                                    string cstatus = GetEMSProjectStatus(itemCRM.project_status.ToLower().Trim());
                                    string cstatusdetail = itemCRM.project_status.Trim();
                                    AddData(pmuid, pmuid, itemCRM.project_id, billteam, itemCRM.estimate_time, itemCRM.start_date.ToString(), itemCRM.primary_technology, itemCRM.client_name, itemCRM.project_name, itemCRM.modelname, itemCRM.developer,
                                       itemCRM.project_id, cstatus, itemCRM.pdev_detail, crmPMEmail, bucketModelDataList, getTechList, cstatusdetail, IsSuperAdmin, itemCRM.other_contact);

                                }
                                catch (Exception ex)
                                {
                                    ManageLog.WriteLogFile("============================== Add New Data (Main Method) for Project Id (" + itemCRM.project_id + ") : Error (" + DateTime.Now.ToString() + ") ==============================");
                                    ManageLog.WriteLogFile((ex.InnerException ?? ex).Message);
                                    ManageLog.WriteLogFile((ex.InnerException ?? ex).StackTrace);
                                    ManageLog.WriteLogFile("============================== /Error ==============================");

                                    //ManageLog.WriteLogFile("============================== Error(" + DateTime.Now.ToString() + ") ==============================");
                                    //ManageLog.WriteLogFile(" Add Project " + itemCRM.project_id + " " + ex.Message);
                                    //ManageLog.WriteLogFile("============================== /Error ==============================");
                                }
                            }
                        }

                        #endregion

                        #region "Close Not Existing Projects of EMS"
                        if (IsLive)
                        {
                            // Get Additional CRM project Ids from EMS
                            var emsAdditionalCRMProjectIds = emsCRMProjectIds.Except(crmProjectIds);

                            if (emsAdditionalCRMProjectIds != null && emsAdditionalCRMProjectIds.Any())
                            {
                                foreach (var projectId in emsAdditionalCRMProjectIds)
                                {
                                    var projectEntity = EMSProjects.FirstOrDefault(x => x.CRMProjectId == Convert.ToInt32(projectId));

                                    if (projectEntity != null && !projectEntity.IsInHouse)
                                    {
                                        projectEntity.Status = "C";
                                        if (projectEntity.ProjectDevelopers != null && projectEntity.ProjectDevelopers.Any())
                                        {
                                            projectEntity.ProjectDevelopers.ToList().ForEach(PD => PD.WorkStatus = (int)Enums.ProjectDevWorkStatus.Closed);
                                        }

                                        projectService.SaveProjectEntity(projectEntity);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Delete Ohere Pm from ProjectOtherPm
                        //Get List of Removed other PM list 
                        var GetRemovedOtherPm = EMSProjectList.Where(x => !CRMProjectList.Select(s => s.project_id).ToList().Contains(x.EmsCRMPId) && x.ProjectOtherContactPMList.Any(pot => pot.PMUid == pmuid)).ToList();

                        foreach (var item in GetRemovedOtherPm)
                        {
                            try
                            {
                                if (pmuid > 0)
                                {
                                    DeleteOtherPm(pmuid, Convert.ToInt32(item.project_id));
                                }

                            }
                            catch (Exception ex)
                            {
                                ManageLog.WriteLogFile("============================== Removed Other Pm for Project Id (" + item.project_id + ") : Error (" + DateTime.Now.ToString() + ") ==============================");
                                ManageLog.WriteLogFile((ex.InnerException ?? ex).Message);
                                ManageLog.WriteLogFile((ex.InnerException ?? ex).StackTrace);
                                ManageLog.WriteLogFile("============================== /Error ==============================");

                            }
                        }
                        #endregion


                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void AddData(int pmuid, int? uid, string projectId, string billteam, string destimate_time, string start_date, string primary_technology, string client_name, string Lprojectname, string Lmodelname, string LDevloper, string Lprojectid,
           string Lstatus, string developerDetails, string pm_Email, List<BucketModel> bucketModelDataList, List<Technology> getTechList, string Lstatusdetail
            , bool IsSuperAdmin, string OtherContact)
        {
            Guid TransId = Guid.NewGuid();
            try
            {
                List<UpdateCRMProjectSchedular.Model.DeveloperDetailIDs> DeveloperDetails = new List<UpdateCRMProjectSchedular.Model.DeveloperDetailIDs>();
                if (!string.IsNullOrEmpty(developerDetails))
                {
                    developerDetails.Split(',').ToList().ForEach(a =>
                    {

                        if (a.Split(':').Length == 2 && (a.Split(':')[1] != "0" || a.Split(':')[1] != ""))
                        {
                            DeveloperDetails.Add(new UpdateCRMProjectSchedular.Model.DeveloperDetailIDs
                            {
                                ActualDeveloperID = a.Split(':')[0] == "0" ? null : Convert.ToInt32(a.Split(':')[0]) as Nullable<Int32>,
                                VirtualDeveloperID = Convert.ToInt32(a.Split(':')[1])
                            });
                        }
                    });
                }
                int estimateTime = 0;
                int.TryParse(destimate_time, out estimateTime);
                Project projectobj = new Project();
                {
                    projectobj.Name = Lprojectname;
                    projectobj.AddDate = DateTime.Now;
                    projectobj.ModifyDate = DateTime.Now;
                    projectobj.CRMProjectId = Convert.ToInt32(Lprojectid);
                    projectobj.BillingTeam = billteam;
                    projectobj.EstimateTime = estimateTime;   //Convert.ToInt16(destimate_time);
                    projectobj.StartDate = Convert.ToDateTime(start_date);
                    projectobj.Uid = uid;




                    if (pm_Email != "")
                    {
                        var abroadPmInfo = userLoginService.GetForgotMailByEmailId(pm_Email);
                        if (abroadPmInfo != null)
                        {
                            projectobj.AbroadPMUid = abroadPmInfo.Uid;
                        }
                    }

                    BucketModel bucktobj = bucketModelDataList.FirstOrDefault(P => P.ModelName.ToLower().Trim().ToLower() == Lmodelname.ToLower().Trim().ToLower());

                    if (client_name != "")
                    {
                        var clientObj = projectService.SaveProjectClient(new Client
                        {
                            Name = client_name,
                            Email = "",
                            Msn = "",
                            Phone = "",
                            IsActive = true,
                            Address = "",
                            ModifyDate = DateTime.Now,
                            AddDate = DateTime.Now,
                            Ip = "",
                            PMUid = pmuid
                        });
                        projectobj.ClientId = clientObj.ClientId;
                    }

                    if (bucktobj != null)
                    {
                        projectobj.Model = bucktobj.BucketId;
                    }
                    else
                    {
                        BucketModelDto newbuckt = new BucketModelDto();
                        newbuckt.ModelName = Lmodelname;
                        bucketModelService.Save(newbuckt);
                    }

                    projectobj.ActualDevelopers = Convert.ToInt32(LDevloper);
                    projectobj.Status = Lstatus; // GetEMSProjectStatus(Lstatus.ToLower().Trim());                   
                    projectobj.PMUid = pmuid;
                    projectobj.IP = ""; // Common.getip();
                                        //projectobj.Save();

                    if (OtherContact.ToLower() == "yes")
                    {
                        projectobj.ProjectOtherPm.Add(
                            new ProjectOtherPm
                            {
                                Pmuid = pmuid
                            });
                    }


                    projectService.Save(projectobj);
                }

                string GetId = string.Empty;
                string DepartmentId = string.Empty;

                string[] Tech_arr = new string[] { };

                Tech_arr = (!String.IsNullOrEmpty(primary_technology) ? primary_technology.ToLower().Trim().Split(',') : null);

                if (Tech_arr != null && Tech_arr.Count() > 0)
                {
                    Tech_arr = Tech_arr.Select(l => String.Join(" ", l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))).ToArray();
                    string[] distinctArray = RemoveDuplicates(Tech_arr);

                    foreach (var TectList in getTechList)
                    {
                        if (TectList != null)
                        {
                            var Techresults = Array.FindAll(distinctArray, S => S.Equals(TectList.Title.ToLower().Trim()) || (TectList.Title.ToLower().Trim()).Contains(S));
                            if (Techresults != null && Techresults.Length > 0)
                            {
                                string Techname = Techresults[0].ToString();
                                GetId += technologyService.GetTechnologyByName(Techname).TechId + ",";

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
                                //if (!GroupId.Contains(Enums.ProjectGroup.Designer.ToString()))
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
                            projectService.SaveProjectTech(new Project_Tech
                            {
                                ProjectID = projectobj.ProjectId,
                                TechId = Convert.ToInt32(ldistinctArray[id])
                            });
                        }

                    }
                    string[] project_DId = (!String.IsNullOrEmpty(DepartmentId) ? DepartmentId.TrimEnd(',').Split(',') : null);
                    string[] pdistinctArray = Department_RemoveDuplicates(project_DId);

                    if (pdistinctArray != null && pdistinctArray.Count() > 0)
                    {
                        for (int DID = 0; DID <= pdistinctArray.ToList().Count - 1; DID++)
                        {
                            projectService.SaveProjectDept(new Project_Department
                            {
                                ProjectID = projectobj.ProjectId,
                                DeptID = Convert.ToInt32(pdistinctArray[DID])
                            });
                        }
                        projectService.SaveProjectDept(new Project_Department
                        {
                            ProjectID = projectobj.ProjectId,
                            DeptID = (int)Enums.ProjectDepartment.WebDesigning
                        });
                    }
                }

                foreach (var dev in DeveloperDetails)
                {
                    if (dev.VirtualDeveloperID != 0)
                    {

                        projectService.SaveProjectDeveloper(new ProjectDeveloper
                        {
                            ProjectId = Convert.ToInt32(projectobj.ProjectId),
                            Uid = dev.ActualDeveloperID,
                            TransId = TransId,
                            Remark = "From CRM WebServics",
                            WorkStatus = (int)Enums.ProjectDevWorkStatus.Running,
                            //WorkRole = (int)Enums.ProjectDevWorkRole.Paid,
                            VD_id = dev.VirtualDeveloperID,
                            AddDate = DateTime.Now,
                            IP = "" //Common.getip()
                        });

                    }
                }
                if (projectobj.Status == "H" || projectobj.Status == "C" || projectobj.Status == "D")
                {
                    if (projectobj.ProjectDevelopers != null && projectobj.ProjectDevelopers.Count > 0)
                    {
                        projectobj.ProjectDevelopers.ToList().ForEach(PD => PD.WorkStatus = (int)Enums.ProjectDevWorkStatus.Closed);
                        //projectobj.Save();
                        projectService.SaveProjectEntity(projectobj);
                    }

                }
                if (IsSuperAdmin)
                {
                    UpdateProjectStatus(Convert.ToInt32(Lprojectid), Lstatusdetail);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.ToString().Contains("Violation of UNIQUE KEY constraint"))
                    {
                        ManageLog.WriteLogFile("============================== Add Data for Project Id (" + projectId + ") : Error => Project name already exists, please try another name (" + DateTime.Now.ToString() + ") ==============================");
                        //ManageLog.WriteLogFile("============================== Error(" + DateTime.Now.ToString() + ") ==============================");
                        //ManageLog.WriteLogFile("Error: Project name already exists, please try another name");
                        //ManageLog.WriteLogFile("============================== /Error ==============================");
                    }
                    else if (ex.InnerException.ToString().Contains("The INSERT statement conflicted with the FOREIGN KEY constraint"))
                    {
                        ManageLog.WriteLogFile("============================== Add Data for Project Id (" + projectId + ") : Error => Insert statement conflicted with the FOREIGN KEY constraint (" + DateTime.Now.ToString() + ") ==============================");
                        //ManageLog.WriteLogFile("============================== Error(" + DateTime.Now.ToString() + ") ==============================");
                        //ManageLog.WriteLogFile("Error: Insert statement conflicted with the FOREIGN KEY constraint");
                        //ManageLog.WriteLogFile("============================== /Error ==============================");
                    }
                    else
                    {
                        ManageLog.WriteLogFile("============================== Add Data for Project Id (" + projectId + ") : Error (" + DateTime.Now.ToString() + ") ==============================");
                        ManageLog.WriteLogFile((ex.InnerException ?? ex).Message);
                        ManageLog.WriteLogFile("============================== /Error ==============================");
                    }
                    return;
                }
                else
                {
                    ManageLog.WriteLogFile("============================== Add Data for Project Id (" + projectId + ") : Error (" + DateTime.Now.ToString() + ") ==============================");
                    ManageLog.WriteLogFile((ex.InnerException ?? ex).Message);
                    ManageLog.WriteLogFile("============================== /Error ==============================");
                    return;
                }
                //TraceError.ReportError(ex, SiteKey.To, Request.Url.PathAndQuery);

            }
            ManageLog.WriteLogFile("============================== Add Data for Project Id (" + projectId + "), Completed (" + DateTime.Now.ToString() + ") ==============================");
        }

        public static void UpdateData(int pmuid, int? uid, string ProjectId, string billteam, string estimate_time, string start_date, string primary_technology, string client_name, string Lprojectname, string Lmodelname, string LDevloper, string Lprojectid,
           string Lstatus, string developerDetails, string pm_Email, List<BucketModel> bucketModelDataList, List<Technology> getTechList, string Lstatusdetail
            , bool IsSuperAdmin, string other_contact)
        {
            // ManageLog.WriteLogFile("============================== Update Data for Project Manager(" + pmuid + ") Started (" + DateTime.Now.ToString() + ") ==============================");
            Guid TransId = Guid.NewGuid();
            try
            {

                //  Project projectEntity = projectService.GetProjectByCRMId(Convert.ToInt32(ProjectId), pmuid);
                Project projectEntity = projectService.GetProjectByCRMId(Convert.ToInt32(ProjectId));

                if (projectEntity != null && !projectEntity.IsInHouse)
                {
                    List<Model.DeveloperDetailIDs> DeveloperDetails = new List<Model.DeveloperDetailIDs>();
                    if (!string.IsNullOrEmpty(developerDetails))
                    {
                        developerDetails.Split(',').ToList().ForEach(a =>
                        {

                            if (a.Split(':').Length == 2 && (a.Split(':')[1] != "0" || a.Split(':')[1] != ""))
                            {
                                DeveloperDetails.Add(new Model.DeveloperDetailIDs
                                {
                                    ActualDeveloperID = a.Split(':')[0] == "0" ? null : Convert.ToInt32(a.Split(':')[0]) as Nullable<Int32>,
                                    VirtualDeveloperID = Convert.ToInt32(a.Split(':')[1])
                                });
                            }
                        });
                    }

                    BucketModel bucktobj = bucketModelDataList.FirstOrDefault(P => P.ModelName.ToLower().Trim() == Lmodelname.ToLower().Trim());
                    if (bucktobj != null)
                    {
                        projectEntity.Model = bucktobj.BucketId;
                        projectEntity.Status = Lstatus;
                        projectEntity.ActualDevelopers = Convert.ToInt32(LDevloper);
                        projectEntity.BillingTeam = billteam;
                        projectEntity.StartDate = Convert.ToDateTime(start_date);
                        projectEntity.ModifyDate = DateTime.Now;
                        projectEntity.Uid = uid; // SiteSession.SessionUser.Uid;

                        projectEntity.AbroadPMUid = null;

                        if (pm_Email != "")
                        {
                            var abroadPmInfo = userLoginService.GetForgotMailByEmailId(pm_Email);
                            if (abroadPmInfo != null)
                            {
                                projectEntity.AbroadPMUid = abroadPmInfo.Uid;
                            }
                        }

                        if (projectEntity.ClientId == null && client_name != "")
                        {
                            if (client_name != "")
                            {
                                var clientObj = projectService.SaveProjectClient(new Client
                                {
                                    Name = client_name,
                                    Email = "",
                                    Msn = "",
                                    Phone = "",
                                    IsActive = true,
                                    Address = "",
                                    ModifyDate = DateTime.Now,
                                    AddDate = DateTime.Now,
                                    Ip = "",
                                    PMUid = pmuid
                                });
                                projectEntity.ClientId = clientObj.ClientId;
                            }

                        }

                        var project_techlist = projectService.GetDataProjectTechByProjectId(projectEntity.ProjectId);
                        if (project_techlist.Count == 0)
                        {
                            string GetId = string.Empty;
                            string DepartmentId = string.Empty;

                            string[] Tech_arr = new string[] { };

                            Tech_arr = (!String.IsNullOrEmpty(primary_technology) ? primary_technology.ToLower().Trim().Split(',') : null);


                            if (Tech_arr != null && Tech_arr.Count() > 0)
                            {
                                Tech_arr = Tech_arr.Select(l => String.Join(" ", l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))).ToArray();
                                string[] distinctArray = RemoveDuplicates(Tech_arr);

                                foreach (var TectList in getTechList)
                                {
                                    if (TectList != null)
                                    {
                                        var Techresults = Array.FindAll(distinctArray, S => S.Equals(TectList.Title.ToLower().Trim()) || (TectList.Title.ToLower().Trim()).Contains(S));
                                        if (Techresults.Length > 0)
                                        {

                                            string Techname = Techresults[0].ToString();
                                            GetId += technologyService.GetTechnologyByName(Techname).TechId + ",";

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
                                string[] List_Technology = (!String.IsNullOrEmpty(GetId) ? GetId.TrimEnd(',').Split(',') : null);
                                string[] ldistinctArray = Tech_RemoveDuplicates(List_Technology);
                                if (ldistinctArray != null && ldistinctArray.Count() > 0)
                                {
                                    for (int id = 0; id <= ldistinctArray.ToList().Count - 1; id++)
                                    {
                                        Project_Tech projecttechobj = new Project_Tech();
                                        projecttechobj.ProjectID = projectEntity.ProjectId; ;
                                        projecttechobj.TechId = Convert.ToInt32(ldistinctArray[id]);
                                        projectService.SaveProjectTech(projecttechobj);
                                    }

                                }
                                var Department_List = projectService.GetDataProjectTechByProjectId(projectEntity.ProjectId);
                                if (Department_List.Count == 0)
                                {

                                    string[] project_DId = (!String.IsNullOrEmpty(DepartmentId) ? DepartmentId.TrimEnd(',').Split(',') : null);
                                    string[] pdistinctArray = Department_RemoveDuplicates(project_DId);
                                    if (pdistinctArray != null && pdistinctArray.Count() > 0)
                                    {
                                        for (int GID = 0; GID <= pdistinctArray.ToList().Count - 1; GID++)
                                        {
                                            projectService.SaveProjectDept(new Project_Department
                                            {
                                                ProjectID = projectEntity.ProjectId,
                                                DeptID = Convert.ToInt32(pdistinctArray[GID])
                                            });
                                        }
                                        projectService.SaveProjectDept(new Project_Department
                                        {
                                            ProjectID = projectEntity.ProjectId,
                                            DeptID = (int)Enums.ProjectDepartment.WebDesigning
                                        });
                                        projectService.SaveProjectDept(new Project_Department
                                        {
                                            ProjectID = projectEntity.ProjectId,
                                            DeptID = (int)Enums.ProjectDepartment.QualityAnalyst
                                        });
                                    }
                                }
                            }
                        }
                        if (projectEntity.ProjectDevelopers != null && DeveloperDetails.Count() > 0)
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
                                    }
                                    else
                                    {
                                        projectService.SaveProjectDeveloper(new ProjectDeveloper
                                        {
                                            ProjectId = Convert.ToInt32(projectEntity.ProjectId),
                                            Uid = dev.ActualDeveloperID,
                                            TransId = TransId,
                                            Remark = "From CRM WebServics",
                                            WorkStatus = (int)Enums.ProjectDevWorkStatus.Running,
                                            VD_id = dev.VirtualDeveloperID,
                                            AddDate = DateTime.Now,
                                            IP = ""
                                        });
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
                        if (other_contact.ToLower() == "no")
                        {
                            projectEntity.PMUid = pmuid;

                            DeleteOtherPm(pmuid, projectEntity.ProjectId);
                        }

                        // Update PMUID in ProjectOtherPM table if other_contact="yes" coming from CRM API
                        if (other_contact.ToLower() == "yes")
                        {
                            if (!projectEntity.ProjectOtherPm.Any(x => x.ProjectId == projectEntity.ProjectId && x.Pmuid == pmuid))
                            {
                                projectEntity.ProjectOtherPm.Add(
                                   new ProjectOtherPm
                                   {
                                       Pmuid = pmuid
                                   });
                                //  projectService.SaveProjectEntity(projectEntity);
                            }
                        }


                        projectService.SaveProjectEntity(projectEntity);
                    }
                    else
                    {
                        ManageLog.WriteLogFile("============================== Update Data for Project Id (" + ProjectId + ") : Error => Model does't define in crm so record will not be updated (" + DateTime.Now.ToString() + ") ==============================");
                        //ManageLog.WriteLogFile("Error: Model does't define in crm so record will not be updated.");
                        //ManageLog.WriteLogFile("============================== /Error ==============================");
                        return;
                    }
                    if (IsSuperAdmin)
                    {
                        UpdateProjectStatus(projectEntity.CRMProjectId, Lstatusdetail);
                    }
                }
            }
            catch (Exception ex)
            {
                ManageLog.WriteLogFile("============================== Update Data for Project Id (" + ProjectId + ") : Error (" + DateTime.Now.ToString() + ") ==============================");
                ManageLog.WriteLogFile((ex.InnerException ?? ex).Message);
                ManageLog.WriteLogFile("============================== /Error ==============================");
                //TraceError.ReportError(ex, AppKeys.To, "Crm Project Schedular");
                return;
            }
            ManageLog.WriteLogFile("============================== Update Data for Project Id (" + ProjectId + ") Completed (" + DateTime.Now.ToString() + ") ==============================");
        }

        public static string[] Department_RemoveDuplicates(string[] project_DId)
        {
            if (project_DId != null)
            {
                return project_DId.Distinct().ToArray<string>();
            }
            else { return null; }
        }

        public static string[] RemoveDuplicates(string[] Tech_arr)
        {
            if (Tech_arr != null)
            {
                return Tech_arr.Distinct().ToArray<string>();
            }
            else { return null; }


        }

        public static string[] Tech_RemoveDuplicates(string[] List_Technology)
        {
            if (List_Technology != null)
            {
                return List_Technology.Distinct().ToArray<string>();
            }
            else { return null; }
        }

        public static void UpdateProjectStatus(int crmId, string status)
        {
            try
            {
                if (crmId > 0)
                {
                    ProjectStatusDto model = new ProjectStatusDto();
                    model.CRMID = crmId;
                    model.Status = status;//Running,Runing,over run,over-run,on hold,hold,complete,completed,remove,deactive,not initiated
                    model.StatusDate = DateTime.Now;

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://ds212.projectstatus.co.uk:8256/");
                    client.DefaultRequestHeaders
                          .Accept
                          .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "api/updatestatus");
                    request.Content = new StringContent(JsonConvert.SerializeObject(model),
                                                        Encoding.UTF8,
                                                        "application/json");//CONTENT-TYPE header

                    client.SendAsync(request)
                          .ContinueWith(responseTask =>
                          {
                              Console.WriteLine("Response: {0}", responseTask.Result);
                          });

                }
            }
            catch (Exception ex)
            {

            }
        }

        public static string GetEMSProjectStatus(string CRMProjectStatus)
        {
            string status = string.Empty;
            switch (CRMProjectStatus)
            {
                case "running":
                case "runing":
                    status = "R";
                    break;
                case "over run":
                case "over-run":
                    status = "O";
                    break;
                case "on hold":
                case "hold":
                    status = "H";
                    break;
                case "complete":
                case "completed":
                    status = "C";
                    break;
                case "remove":
                case "deactive":
                    status = "D";
                    break;
                case "not initiated":
                    status = "I";
                    break;
                case "not converted":
                    status = "N";
                    break;
            }

            return status;
        }

        public static string GetBillingTeam(string BillingTeam)
        {
            string BillTeam = string.Empty;
            switch (BillingTeam)
            {

                case "aus":
                    BillTeam = "AUS";
                    break;
                case "uk":
                    BillTeam = "UK";
                    break;
                case "us":
                    BillTeam = "USA";
                    break;
                case "australia":
                    BillTeam = "AUS";
                    break;
                case "india fl":
                    BillTeam = "INRFL";
                    break;
                case "india":
                    BillTeam = "INR";
                    break;
            }
            return BillTeam;
        }


        public static void DeleteOtherPm(int PmUid, int Project_Id)
        {

            var otherpm = projectService.GetProjectOtherPmsByPmuidAndProjectId(PmUid, Project_Id);
            if (otherpm.Count > 0)
                projectService.DeleteProjectOtherPms(otherpm);

        }
    }

}

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
using EMS.API.Model;
using ProjectOtherContactPM = EMS.API.Model.ProjectOtherContactPM;
using EMS.API.LIBS;

namespace EMS.API.DAL
{
    public class CRMData : ICRMData
    {
        private IProjectService projectService;
        private IUserLoginService userLoginService;
        private IVirtualDeveloperService virtualDeveloperService;
        private IBucketModelService bucketModelService;
        private ITechnologyService technologyService;
        private IManageLog serviceManageLog;


        static List<ProjectInfoModel> Lsproject = new List<ProjectInfoModel>();

        public CRMData(IProjectService _projectService,
        IUserLoginService _userLoginService,
        IVirtualDeveloperService _virtualDeveloperService,
        IBucketModelService _bucketModelService,
        ITechnologyService _technologyService,
         IManageLog _serviceManageLog)
        {

            projectService = _projectService;
            userLoginService = _userLoginService;
            virtualDeveloperService = _virtualDeveloperService;
            bucketModelService = _bucketModelService;
            technologyService = _technologyService;
            serviceManageLog = _serviceManageLog;
        }

        public ResponseModel<string> UpdateCRMData(int pmuid, ProjectInfoReqModel CrmProjectModel)
        {
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {

                #region "Get EMS Projects"

                BucketModel bucktobj = bucketModelService.GetData().FirstOrDefault(P => P.ModelName.ToLower().Trim().ToLower() == CrmProjectModel.modelname.ToLower().Trim().ToLower());

                if (bucktobj != null)
                {
                    //Get List Of technologies 

                    var getTechList = technologyService.GetTechnologyList();
                    
                    List<UserLogin> userlist = userLoginService.GetUsers(true) ?? new List<UserLogin>();


                    //Get List Of  Virtual Developers related to PmId
                    List<VirtualDeveloper> virtualDeveloperList = virtualDeveloperService.GetVirtualDeveloperByPMUid(pmuid) ?? new List<VirtualDeveloper>();

                    string crmPMEmail = string.IsNullOrEmpty(CrmProjectModel.pm_email) ? "" : Convert.ToString(CrmProjectModel.pm_email);

                    //List of Billing team by Pmid
                    string Bteam = GetBillingTeam(CrmProjectModel.bill_team.ToLower().Trim());

                    //Get project current status 
                    string cstatus = GetEMSProjectStatus(CrmProjectModel.project_status.ToLower().Trim());


                    string cstatusdetail = CrmProjectModel.project_status.Trim();


                    string actual_dev_email = string.Empty;
                    string virtual_dev_email = string.Empty;
                    string actual_devID = string.Empty;
                    string virtual_devID = string.Empty;
                    string strdevdetails = "";


                    // Filter list of Actual & Virtual developer Ids 
                    if (CrmProjectModel.dev_detail != null && CrmProjectModel.dev_detail.Count > 0)
                    {
                        for (int i = 0; i <= CrmProjectModel.dev_detail.Count - 1; i++)
                        {
                            int actid = 0;
                            int vdevid = 0;
                            actual_dev_email = string.Empty;

                            if (CrmProjectModel.dev_detail[i].actual != null)
                            {
                                try
                                {
                                    actual_dev_email = (CrmProjectModel.dev_detail[i].actual.email ?? "").ToLower().Trim();
                                }
                                catch
                                {
                                    actual_dev_email = "";
                                }
                            }

                            virtual_dev_email = string.Empty;

                            if (CrmProjectModel.dev_detail[i].virtual_dev != null)
                            {
                                try
                                {
                                    virtual_dev_email = (CrmProjectModel.dev_detail[i].virtual_dev.email ?? "").ToLower().Trim();
                                }
                                catch
                                {
                                    virtual_dev_email = "";
                                }
                            }

                            if (userlist.Count > 0 && virtualDeveloperList.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(actual_dev_email) && userlist.Select(X => X.EmailOffice.ToLower()).Contains(actual_dev_email) == true)
                                {
                                    var acdevid = userlist.FirstOrDefault(R => R.EmailOffice.ToLower().Trim() == actual_dev_email && R.IsActive == true);
                                    actual_devID += (acdevid != null ? (acdevid.Uid.ToString() + ",") : "");
                                    actid = (acdevid != null ? acdevid.Uid : 0);
                                }

                                if (!string.IsNullOrEmpty(virtual_dev_email))
                                {
                                    bool isvDev = virtualDeveloperList.Any(a => a.emailid.ToLower().Trim() == virtual_dev_email && a.isactive == true);
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

                    #endregion


                    //Set Actual/virtual developer in developer entity
                    Guid TransId = Guid.NewGuid();

                    Project projectEntity = projectService.GetProjectByCRMId(Convert.ToInt32(CrmProjectModel.project_id)) ?? new Project();

                    if (projectEntity != null)
                    {

                        List<Model.DeveloperDetailIDs> DeveloperDetails = new List<Model.DeveloperDetailIDs>();


                        if (!string.IsNullOrEmpty(strdevdetails))
                        {
                            strdevdetails.Split(',').ToList().ForEach(a =>
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


                        int.TryParse(CrmProjectModel.estimate_time, out int estimateTime);



                        projectEntity.Status = cstatus;
                        projectEntity.ActualDevelopers = Convert.ToInt32(CrmProjectModel.developer);
                        projectEntity.BillingTeam = Bteam;
                        projectEntity.StartDate = Convert.ToDateTime(CrmProjectModel.start_date);
                        projectEntity.ModifyDate = DateTime.Now;
                        projectEntity.Uid = pmuid; // SiteSession.SessionUser.Uid;
                        projectEntity.EstimateTime = estimateTime;

                        projectEntity.PMUid = pmuid;

                        projectEntity.Name = CrmProjectModel.project_name;

                        projectEntity.Model = bucktobj.BucketId;

                        projectEntity.AbroadPMUid = null;

                        // Update Abroad PM user
                        if (crmPMEmail != "")
                        {
                            var abroadPmInfo = userLoginService.GetForgotMailByEmailId(crmPMEmail);
                            if (abroadPmInfo != null)
                            {
                                projectEntity.AbroadPMUid = abroadPmInfo.Uid;
                            }
                        }


                        // Insert project client name in table if it is not added into system
                        if (projectEntity.ClientId == null && CrmProjectModel.client_name != "")
                        {
                            if (CrmProjectModel.client_name != "")
                            {
                                var clientObj = projectService.SaveProjectClient(new Client
                                {
                                    Name = CrmProjectModel.client_name,
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



                        if (projectEntity.ProjectId > 0)
                        {

                            //Remove all existing Project Technologies and

                            if (projectEntity.Project_Tech != null)
                            {
                                projectService.ChangeEntityCollectionStateAsDelete(projectEntity.Project_Tech);
                                projectEntity.Project_Tech.Clear();
                            }

                            //Remove all existing project Project_Department 

                            if (projectEntity.Project_Department != null)
                            {
                                projectService.ChangeEntityCollectionStateAsDelete(projectEntity.Project_Department);
                                projectEntity.Project_Department.Clear();
                            }

                            //Remove all existing project developers 

                            //if (projectEntity.ProjectDevelopers != null)
                            //{
                            //    projectService.ChangeEntityCollectionStateAsDelete(projectEntity.ProjectDevelopers);
                            //    projectEntity.ProjectDevelopers.Clear();
                            //}

                            //Remove all existing ProjectOtherPm  
                            if (projectEntity.ProjectOtherPm != null)
                            {
                                projectService.ChangeEntityCollectionStateAsDelete(projectEntity.ProjectOtherPm);
                                projectEntity.ProjectOtherPm.Clear();
                            }


                        }

                        // Get technology and departname according to technology names
                        string[] Tech_arr = new string[] { };
                        Tech_arr = (!String.IsNullOrEmpty(CrmProjectModel.primary_technology) ? CrmProjectModel.primary_technology.ToLower().Trim().Split(',') : null);
                        if (Tech_arr != null && Tech_arr.Count() > 0)
                        {

                            string GetId = string.Empty;
                            string DepartmentId = string.Empty;
                            var TechList = technologyService.GetTechnologyList();

                            Tech_arr = Tech_arr.Select(l => String.Join(" ", l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))).ToArray();
                            string[] distinctArray = RemoveDuplicates(Tech_arr);

                            foreach (var Techname in distinctArray)
                            {
                                var Techresults = TechList.FirstOrDefault(x => x.Title.ToLower().Contains(Techname.ToLower()));
                                if (Techresults != null)
                                {
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



                            // Add Project Technologies
                            string[] List_Technology = (!String.IsNullOrEmpty(GetId) ? GetId.TrimEnd(',').Split(',') : null);
                            string[] ldistinctArray = Tech_RemoveDuplicates(List_Technology);

                            if (ldistinctArray != null && ldistinctArray.Count() > 0)
                            {
                                for (int id = 0; id <= ldistinctArray.ToList().Count - 1; id++)
                                {
                                    projectEntity.Project_Tech.Add(new Project_Tech()
                                    {
                                        TechId = Convert.ToInt32(ldistinctArray[id])
                                    });
                                }

                            }

                            // Add Project Project_Department
                            string[] project_DId = (!String.IsNullOrEmpty(DepartmentId) ? DepartmentId.TrimEnd(',').Split(',') : null);
                            string[] pdistinctArray = Department_RemoveDuplicates(project_DId);
                            if (pdistinctArray != null && pdistinctArray.Count() > 0)
                            {
                                for (int GID = 0; GID <= pdistinctArray.ToList().Count - 1; GID++)
                                {
                                    projectEntity.Project_Department.Add(new Project_Department
                                    {
                                        DeptID = Convert.ToInt32(pdistinctArray[GID])
                                    });

                                }
                                projectEntity.Project_Department.Add(new Project_Department
                                {
                                    DeptID = (int)Enums.ProjectDepartment.WebDesigning
                                });
                                projectEntity.Project_Department.Add(new Project_Department
                                {
                                    DeptID = (int)Enums.ProjectDepartment.QualityAnalyst
                                });
                            }

                        }


                        //  Add/Update developer
                        //if (projectEntity.ProjectDevelopers != null && DeveloperDetails.Count() > 0)
                        //{
                        //    foreach (var dev in DeveloperDetails)
                        //    {
                        //        if (dev.VirtualDeveloperID != 0)
                        //        {

                        //            projectEntity.ProjectDevelopers.Add(new ProjectDeveloper
                        //            {
                        //                //ProjectId = projectEntity.ProjectId,
                        //                Uid = dev.ActualDeveloperID,
                        //                TransId = TransId,
                        //                Remark = "From CRM WebServics",
                        //                WorkStatus = (int)Enums.ProjectDevWorkStatus.Running,
                        //                VD_id = dev.VirtualDeveloperID,
                        //                AddDate = DateTime.Now,
                        //                IP = ""
                        //            });
                        //        }
                        //    }
                        //}
                        //  Add/Update developer
                        if (DeveloperDetails.Count() > 0)
                        {

                            var projectdevList = projectEntity.ProjectDevelopers.ToList();//.FindAll(X => X.ProjectId == projectEntity.ProjectId);
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
                                        projectEntity.ProjectDevelopers.Add(new ProjectDeveloper
                                        {
                                            //ProjectId = Convert.ToInt32(projectEntity.ProjectId),
                                            Uid = dev.ActualDeveloperID,
                                            TransId = TransId,
                                            Remark = "From CRM WebServics",
                                            WorkStatus = (int)Enums.ProjectDevWorkStatus.Running,
                                            VD_id = dev.VirtualDeveloperID,
                                            AddDate = DateTime.Now,
                                            ModifyDate = DateTime.Now,
                                            IP = ""
                                        });
                                    }
                                }
                            }
                        }

                        // If project status is Hold, Complete, Deactivate and Not Initiate then we will closed all the developers of that project
                        if (projectEntity.Status == "H" || projectEntity.Status == "C" || projectEntity.Status == "D" || projectEntity.Status == "N")
                        {
                            if (projectEntity.ProjectDevelopers != null && projectEntity.ProjectDevelopers.Any())
                            {
                                projectEntity.ProjectDevelopers.ToList().ForEach(PD => PD.WorkStatus = (int)Enums.ProjectDevWorkStatus.Closed);
                                //foreach()
                            }
                        }



                        // Add ProjectOtherPm
                        foreach (var item in CrmProjectModel.other_contact)
                        {
                            var otherPM = userlist.FirstOrDefault(x => x.EmailOffice.ToLower() == item.email.ToLower());
                            if (otherPM != null)
                            {

                                projectEntity.ProjectOtherPm.Add(
                                   new ProjectOtherPm
                                   {
                                       Pmuid = otherPM.Uid
                                   });
                            }
                        }
                        


                        if (projectEntity.ProjectId > 0)
                        {
                            projectService.SaveProjectEntity(projectEntity);
                            response.Status = true;
                            response.Message = $"Project has been updated successfully for project CRMId ({ CrmProjectModel.project_id })";
                        }
                        else
                        {
                            projectEntity.CRMProjectId = Convert.ToInt32(CrmProjectModel.project_id);
                            projectEntity.AddDate = DateTime.Now;

                            projectService.Save(projectEntity);

                            response.Status = true;
                            response.Message = $"Project has been added successfully for project CRMId ({  CrmProjectModel.project_id })";
                        }

                        //if (IsSuperAdmin)
                        //{
                        //    UpdateProjectStatus(projectEntity.CRMProjectId, Lstatusdetail);
                        //}
                    }

                  
                }
                else
                {
                    response.Status = false;
                    response.Message = $"Error => Model Name does't define in crm so record will not be updated for project CRMId ({ CrmProjectModel.project_id })";
                }


            }
            catch (Exception ex)
            {
                throw;
            }

            return response;
        }



       

        public string[] Department_RemoveDuplicates(string[] project_DId)
        {
            if (project_DId != null)
            {
                return project_DId.Distinct().ToArray<string>();
            }
            else { return null; }
        }

        public string[] RemoveDuplicates(string[] Tech_arr)
        {
            if (Tech_arr != null)
            {
                return Tech_arr.Distinct().ToArray<string>();
            }
            else { return null; }


        }

        public string[] Tech_RemoveDuplicates(string[] List_Technology)
        {
            if (List_Technology != null)
            {
                return List_Technology.Distinct().ToArray<string>();
            }
            else { return null; }
        }

        public void UpdateProjectStatus(int crmId, string status)
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

        public string GetEMSProjectStatus(string CRMProjectStatus)
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

        public string GetBillingTeam(string BillingTeam)
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
                case "uae":
                    BillTeam = "UAE";
                    break;
            }
            return BillTeam;
        }


        public void DeleteOtherPm(int PmUid, int Project_Id)
        {

            var otherpm = projectService.GetProjectOtherPmsByPmuidAndProjectId(PmUid, Project_Id);
            if (otherpm.Count > 0)
                projectService.DeleteProjectOtherPms(otherpm);

        }
    }

}

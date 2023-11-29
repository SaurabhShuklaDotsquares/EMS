using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Models.Others;
using System;
using System.Collections.Generic;

//using System.Configuration;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using EMS.Website.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Text;
using EMS.Web.Modals;
//using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using EMS.Website.Code.LIBS;
using System.Data;
using NPOI.HSSF.Record.Formula.Functions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using OfficeOpenXml.FormulaParsing.Utilities;
using static EMS.Core.Enums;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;


//using Castle.Core.Configuration;

namespace EMS.Web.Controllers
{
    //[CustomAuthorization(IsAshishTeam: true)]
    [CustomAuthorization]
    public class EstimateController : BaseController
    {
        private ILeadServices leadServices;
        private IUserLoginService userLoginServices;
        private ITechnologyService technologyServices;
        private readonly string[] allLeadsUsers;
        private IConfiguration _configuration;
        private readonly IDomainTypeService domainTypeService;
        private readonly IProjectClosureService projectClosureService;
        private readonly IEstimateService estimateService;
        private readonly ITechnologyParentService technologyParentService;
        private readonly ICurrencyService currencyService;
        private readonly IEstimateHostingPackageService estimateHostingPackageService;

        public EstimateController(ILeadServices leadServices, IUserLoginService userLoginServices, ITechnologyService technologyServices, IConfiguration configuraton, IProjectClosureService _projectClosureService, IDomainTypeService _domainTypeService, IEstimateService _estimateService,
            ITechnologyParentService _technologyParentService,
            ICurrencyService _currencyService,
            IEstimateHostingPackageService _estimateHostingPackageService)
        {
            this.leadServices = leadServices;
            this.userLoginServices = userLoginServices;
            this.technologyServices = technologyServices;
            this.domainTypeService = _domainTypeService;
            _configuration = configuraton;
            this.allLeadsUsers = _configuration.GetSection("SiteKeys").GetSection("AccessAllLeads").Value.Split(','); // Configuration[" ConfigurationManager.AppSettings["AccessAllLeads"].Split(',');
            this.projectClosureService = _projectClosureService;
            this.estimateService = _estimateService;
            this.technologyParentService = _technologyParentService;
            this.currencyService = _currencyService;
            this.estimateHostingPackageService = _estimateHostingPackageService;
        }

        [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult Index()
        {
            EstimateViewModel model = new EstimateViewModel();            
            //List<int> selectedRoles = new List<int> { (int)Enums.UserRoles.PM, (int)Enums.UserRoles.BAPreSales, (int)Enums.UserRoles.BAPrePostSales, (int)Enums.UserRoles.DVManagerial, (int)Enums.UserRoles.DVPManagerial, (int)Enums.UserRoles.QAManagerial, (int)Enums.UserRoles.QAPManagerial, (int)Enums.UserRoles.UIUXManagerial, (int)Enums.UserRoles.UIUXDesigner, (int)Enums.UserRoles.DV };

            model.LeadOwners = leadServices.GetTakenUsersByPM(PMUserId).Select(y => new SelectListItem
            {
                Text = y.Name,
                Value = y.Uid.ToString()
            }).OrderBy(t => t.Text).ToList();

            model.LeadStatus = leadServices.GetLeadStatus(null).Where(x => x.StatusId != 24).Select(x => new SelectListItem
            {
                Text = x.StatusName,
                Value = x.StatusId.ToString()
            }).OrderBy(t => t.Text).ToList(); ;

            model.LeadTypes = leadServices.GetLeadType("Lead").Select(x => new SelectListItem
            {
                Text = x.TypeName,
                Value = x.TypeId.ToString()
            }).OrderBy(t => t.Text).ToList();

            model.AssignedFrom = DateTime.Today.AddDays(-30).ToFormatDateString("dd/MM/yyyy");
            model.AssignedTo = DateTime.Today.ToFormatDateString("dd/MM/yyyy");

            return View(model);
        }

        public List<ProjectLead> FilterProjectLeadDetail(IDataTablesRequest request, string txtSearch, string txtAssignedFrom,
            string txtAssignedTo, string drpOwner, string drpStatus, string drpType, string drpClient,
            string[] chkCountry, string[] chkStatus, bool existClient, bool newClient, bool awaitResp,
            bool escalatedClient, bool newConverted, bool existingConvert, bool isPageLoad, out int totalCount, bool isSummaryFilters = false, bool isCovid19 = false)
        {
            var pagingServcices = request != null ? new PagingService<ProjectLead>(request.Start, request.Length) : new PagingService<ProjectLead>(0, int.MaxValue);
            var expr = PredicateBuilder.True<ProjectLead>();

            #region [ Filter ]

            if (CurrentUser.RoleId != (int)Enums.UserRoles.PM && !allLeadsUsers.Contains(Convert.ToString(CurrentUser.Uid)))
            {
                expr = expr.And(e => e.PMID == PMUserId && e.OwnerId == CurrentUser.Uid);
            }

            if (!string.IsNullOrEmpty(txtSearch))
            {
                bool isClientID = txtSearch.All(char.IsDigit);
                if (isClientID)
                {
                    int crmId = Convert.ToInt32(txtSearch);
                    expr = expr.And(e => (e.LeadId.Equals(crmId) || e.LeadCRMId.Equals(txtSearch)));
                }
                else
                {
                    expr = expr.And(e => e.Title.Contains(txtSearch));
                }
            }
            if (!String.IsNullOrEmpty(txtAssignedFrom))
            {
                DateTime assignedFromDate = txtAssignedFrom.ToDateTime("dd/MM/yyyy").Value;
                expr = expr.And(e => e.AssignedDate >= assignedFromDate);
            }

            if (!string.IsNullOrEmpty(txtAssignedTo))
            {
                DateTime assignedToDate = txtAssignedTo.ToDateTime("dd/MM/yyyy").Value;
                expr = expr.And(e => e.AssignedDate <= assignedToDate);
            }

            if (!string.IsNullOrEmpty(drpOwner))
            {
                int ownerId = Convert.ToInt32(drpOwner);
                expr = expr.And(e => e.OwnerId == ownerId
                || e.CommunicatorId == ownerId
                || e.LeadTechnicians.Any(x => x.TechnicianId == ownerId));
            }

            if (!string.IsNullOrEmpty(drpStatus))
            {
                int statusId = Convert.ToInt32(drpStatus);
                expr = expr.And(e => e.Status == statusId);
            }
            if (!string.IsNullOrEmpty(drpType))
            {
                int typeId = Convert.ToInt32(drpType);
                expr = expr.And(e => e.LeadType == typeId);
            }
            if (!string.IsNullOrEmpty(drpClient))
            {
                bool isNewClient = Convert.ToBoolean(Convert.ToInt32(drpClient));
                expr = expr.And(e => e.IsNewClient == isNewClient);
            }
            if (chkCountry != null && chkCountry.Length > 0)
            {
                expr = expr.And(e => chkCountry.Contains(e.AbroadPM.Country));
            }

            if (!isSummaryFilters && !newClient && !existClient && !newConverted && !existingConvert && !escalatedClient && !awaitResp)
            {
                if (chkStatus != null && chkStatus.Length > 0)
                {
                    expr = expr.And(L => ((!String.IsNullOrEmpty(L.Conclusion) && L.Conclusion.IndexOf("Action") < 0 && L.Conclusion.IndexOf("Chase") < 0 && chkStatus.Contains(L.Conclusion))
                                                 || (chkStatus.Contains("Converted") ? L.TypeMaster.TypeName.Equals("Almost Converted") : false)
                                                 || chkStatus.Contains(L.LeadStatu.StatusName)));
                }
                else
                {
                    expr = expr.And(X => X.LeadStatu.StatusName == "Chase Request" || X.LeadStatu.StatusName == "Action Required From (Team)" || X.LeadStatu.StatusName == "Action Required From (Out of India PM)" || X.LeadStatu.StatusName == "Do Not Chase");
                }
            }

            if (newClient)
            {
                expr = expr.And(e => e.IsNewClient);
            }
            else if (existClient)
            {
                expr = expr.And(e => !e.IsNewClient);
            }
            else if (newConverted)
            {
                expr = expr.And(e => e.IsNewClient && (e.Conclusion == "Converted" || e.TypeMaster.TypeName == "Almost Converted"));
            }
            else if (existingConvert)
            {
                expr = expr.And(e => !e.IsNewClient && (e.Conclusion == "Converted" || e.TypeMaster.TypeName == "Almost Converted"));
            }
            else if (escalatedClient)
            {
                expr = expr.And(e => !String.IsNullOrEmpty(e.Conclusion) && e.Conclusion.Contains("Escalated"));
            }
            else if (awaitResp)
            {
                expr = expr.And(e => e.LeadStatu.StatusName.Contains("Action") || e.LeadStatu.StatusName.Contains("Chase"));
            }

            if (isCovid19)
            {
                expr = expr.And(e => e.IsCovid19 == true);
            }
            pagingServcices.Sort = (o) =>
            {
                var sortedColumns = request != null ? request.SortedColumns() : null;
                if (sortedColumns != null && sortedColumns.Any())
                {
                    return o.OrderBy(c => c.LeadId);
                }
                else
                {
                    return o.OrderByDescending(c => c.ModifyDate);
                }
            };

            pagingServcices.Filter = expr;

            #endregion

            List<ProjectLead> response = leadServices.GetLeadsByPaging(out totalCount, pagingServcices);
            return response;
        }

        [HttpPost]
        [CustomAuthorization(IsAshishTeam: true)]
        public IActionResult Index(IDataTablesRequest request, string txtSearch, string txtAssignedFrom,
            string txtAssignedTo, string drpOwner, string drpStatus, string drpType, string drpClient,
            string[] chkCountry, string[] chkStatus, bool existClient, bool newClient, bool awaitResp,
            bool escalatedClient, bool newConverted, bool existingConvert, bool isPageLoad, bool isCovid19)
        {
            int totalCount;
            int currentUserId = CurrentUser.Uid;
            bool isPMUser = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
            bool isAllLeadUser = allLeadsUsers.Contains(Convert.ToString(currentUserId));

            var AlmostConvertedId = leadServices.GetLeadType("Lead").Where(x => x.TypeName == "Almost Converted").FirstOrDefault().TypeId;

            List<ProjectLead> response = FilterProjectLeadDetail(request, txtSearch, txtAssignedFrom,
            txtAssignedTo, drpOwner, drpStatus, drpType, drpClient,
            chkCountry, chkStatus, existClient, newClient, awaitResp,
            escalatedClient, newConverted, existingConvert, isPageLoad, out totalCount, isCovid19: isCovid19);

            var returnResult = response.Select((r, index) => new
            {
                LeadId = r.LeadId,
                rowIndex = (index + 1) + (request.Start),
                crmid = r.LeadCRMId ?? "",
                ClientId = r.LeadClientId.HasValue ? r.LeadClientId.ToString() : "",
                Client = r.LeadClient?.Name ?? "",
                LeadTitle = r.Title ?? "",
                NewClient = r.IsNewClient ? "New Client" : "Existing Client",
                AbroadPM = r.AbroadPM?.Country ?? "",
                GeneratedDate = r.AddDate.ToFormatDateString("MMM, dd yyyy hh:mm tt") ?? "",
                ModifiedDate = r.ModifyDate.ToFormatDateString("MMM, dd yyyy hh:mm tt") ?? "",
                ProposalDocument = r.ProposalDocument ?? "",
                Title = "",
                LastConversation = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault()?.Notes : "",
                LastConversationFull = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault()?.Notes : "",
                showLastConversationInDiv = r.LeadTransactions.Any() ? r.LeadTransactions.OrderByDescending(x => x.AddDate).FirstOrDefault().Notes?.Length > 100 : false,
                EstimateTimeinDay = r.EstimateTimeinDay != null ? WeekAndDay(Convert.ToInt32(r.EstimateTimeinDay)).ToString() : "",
                OwnersName = r.UserLogin1.Name + "/" + r.UserLogin.Name,
                NextChaseDate = r.NextChasedDate.ToFormatDateString("MMM, dd yyyy") ?? "",
                ConversionDate = r.ConversionDate.ToFormatDateString("MMM, dd yyyy") ?? "",
                ShowConversionDate = r.LeadType == AlmostConvertedId,
                Technologies = r.Technologies,
                Source = r.AbroadPM?.Name ?? "",
                AssignedDate = r.AssignedDate.ToFormatDateString("MMM, dd yyyy hh:mm tt"),
                Status = r.LeadStatu?.StatusName ?? "",
                Remark = r.Remark ?? "",
                AllowConclusionAndEdit = isPMUser || isAllLeadUser || r.OwnerId == currentUserId,
                AllowClientAndDelete = isPMUser,
                AllowAction = (isPMUser || isAllLeadUser || r.OwnerId == currentUserId) && (r.LeadStatu.StatusName.Contains("Action") || r.LeadStatu.StatusName.Contains("Chase"))
            });

            return DataTablesJsonResult(totalCount, request, returnResult);
        }

        [HttpPost]
        [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult GetLeadSummary([FromBody]GetLeadSummaryFilterViewModel filter)
        {
            int totalCount;
            List<ProjectLead> projectLeads = FilterProjectLeadDetail(null, filter.txtSearch, filter.txtAssignedFrom,
                                                filter.txtAssignedTo, filter.drpOwner, filter.drpStatus, filter.drpType, filter.drpClient,
                                                filter.chkCountry, null, filter.existClient, filter.newClient, filter.awaitResp,
                                                filter.escalatedClient, filter.newConverted, filter.existingConvert, filter.isPageLoad, out totalCount, true);

            LeadSummaryViewModel leadSummaryVM = null;
            if (filter.isUpdateLeadSummaryTop)
            {
                int newClients = projectLeads.Count(l => l.IsNewClient);
                int existingClient = projectLeads.Count(l => !l.IsNewClient);

                var convertedLeads = projectLeads.FindAll(L => (!String.IsNullOrEmpty(L.Conclusion) && L.Conclusion.Equals("Converted")) || L.TypeMaster.TypeName.Equals("Almost Converted"));
                int totalConvertedClients = convertedLeads.Count;
                int convertedNewClients = convertedLeads.Count(l => l.IsNewClient);
                int convertedExistingClients = totalConvertedClients - convertedNewClients;
                int escalatedLeads = projectLeads.Count(L => !String.IsNullOrEmpty(L.Conclusion) && L.Conclusion.IndexOf("Escalated") >= 0);
                int awaitingResponse = projectLeads.Count(L => L.LeadStatu.StatusName.IndexOf("Action") >= 0 || L.LeadStatu.StatusName.IndexOf("Chase") >= 0);
                double totalLeads = projectLeads.Count;
                double totalConvertedLeads = totalConvertedClients;
                string TotalConversion = totalLeads > 0 ? ((totalConvertedLeads * 100) / totalLeads) != 0 ? ((totalConvertedLeads * 100) / totalLeads).ToString("##.##") + "%" : "0%" : "0%";
                string totalNewLeadConversion = newClients > 0 ? (((double)convertedNewClients * 100) / newClients) != 0 ? (((double)convertedNewClients * 100) / newClients).ToString("##.##") + "%" : "0%" : "0%";

                leadSummaryVM = new LeadSummaryViewModel()
                {
                    TotalClients = projectLeads.Count,
                    NewClients = newClients,
                    ExistingClient = existingClient,
                    TotalConvertedClients = totalConvertedClients,
                    ConvertedNewClients = convertedNewClients,
                    ConvertedExistingClients = convertedExistingClients,
                    EscalatedLeads = escalatedLeads,
                    AwaitingResponseLeads = awaitingResponse,
                    TotalConversion = TotalConversion,
                    TotalNewLeadConversion = totalNewLeadConversion
                };
            }

            var leaduser = leadServices.GetTakenUsersByPM(PMUserId);

            List<LeadDetails> leadDetails = new List<LeadDetails>();
            int i = 0;
            foreach (var lsUser in leaduser)
            {
                var lsLeadFilter = projectLeads.FindAll(L => L.OwnerId == lsUser.Uid || L.CommunicatorId == lsUser.Uid || L.LeadTechnicians.Any(x => x.TechnicianId == lsUser.Uid));

                if (lsLeadFilter.Count > 0)
                {
                    LeadDetails lead = new LeadDetails();

                    lead.TotalExisting = lsLeadFilter.Count(L => !L.IsNewClient);
                    lead.ExistingConversion = lsLeadFilter.Count(L => !L.IsNewClient && ((!String.IsNullOrEmpty(L.Conclusion) && L.Conclusion.Equals("Converted")) || L.TypeMaster.TypeName.Equals("Almost Converted")));

                    lead.TotalNew = lsLeadFilter.Count(L => L.IsNewClient);
                    lead.NewConversion = lsLeadFilter.Count(L => L.IsNewClient && ((!String.IsNullOrEmpty(L.Conclusion) && L.Conclusion.Equals("Converted")) || L.TypeMaster.TypeName.Equals("Almost Converted")));

                    if (lead.TotalNew > 0 || lead.TotalExisting > 0 || lead.NewConversion > 0 || lead.ExistingConversion > 0)
                    {
                        lead.OwnerId = lsUser.Uid;
                        lead.OwnerName = lsUser.Name;

                        lead.TotalConversion = lsLeadFilter.Count;
                        lead.index = i;

                        leadDetails.Add(lead);
                        i++;
                    }
                }
            }

            IDictionary<string, object> additionalparam = new Dictionary<string, object>();
            additionalparam.Add(new KeyValuePair<string, object>("leadSummary", leadSummaryVM));
            additionalparam.Add(new KeyValuePair<string, object>("userLeadSummary", leadDetails));

            return Json(additionalparam);
        }

        [HttpGet]
        [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult EditEstimate(int id)
        {
            LeadDto leadViewModel = new LeadDto();
            return View(leadViewModel);
        }

        [HttpGet]
        //[CustomAuthorization(IsAshishTeam: true)]
        public ActionResult AddClient(int id)
        {
            LeadClientDto leadClientViewModel = new LeadClientDto();
            leadClientViewModel.LeadId = id;
            int pmUid = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.RoleId == (int)Enums.UserRoles.PMO ? CurrentUser.Uid : CurrentUser.PMUid;
            leadClientViewModel.LeadClientList = leadServices.GetLeadClient(pmUid).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.LeadClientId.ToString()
            }).ToList();
            return PartialView("_LeadClients", leadClientViewModel);
        }

        [HttpPost]
        //[CustomAuthorization(IsAshishTeam: true)]
        public ActionResult AddClient(LeadClientDto leadClientViewModel)
        {
            if (leadClientViewModel != null)
            {
                LeadClient leadClient = new LeadClient();

                if (leadClientViewModel.ClientId == 0)
                {
                    int pmUid = CurrentUser.RoleId == (int)Enums.UserRoles.PM ? CurrentUser.Uid : CurrentUser.RoleId == (int)Enums.UserRoles.PMO ? CurrentUser.Uid : CurrentUser.PMUid;

                    leadClient.Name = leadClientViewModel.Name;
                    leadClient.Email = leadClientViewModel.Email;
                    leadClient.PMUid = pmUid;

                    leadServices.SaveLeadClient(leadClient);
                }

                ProjectLead projectLead = new ProjectLead();
                projectLead = leadServices.GetLeadById(leadClientViewModel.LeadId);

                if (projectLead != null)
                {
                    projectLead.LeadClientId = leadClientViewModel.ClientId == 0 ? leadClient.LeadClientId : leadClientViewModel.ClientId;
                    projectLead.IsNewClient = leadClientViewModel.ClientId == 0 ? true : false;
                    projectLead.ModifyDate = DateTime.Now;
                }
                leadServices.SaveLead(projectLead);
            }

            ShowSuccessMessage("Success", "Lead client has been successfully added to project lead", false);
            return Json(new { isSuccess = true, redirectUrl = Url.Action("index") });
        }

        [HttpGet]
        //[CustomAuthorization(IsAshishTeam: true)]
        public ActionResult Conclusion(int id)
        {
            ProjectLead projectLead = new ProjectLead();
            ConclusionDto conclusionViewModel = new ConclusionDto();
            projectLead = leadServices.GetLeadById(id);
            if (projectLead != null)
            {
                conclusionViewModel.Communicator = projectLead.UserLogin1.Name;
                conclusionViewModel.Owner = projectLead.UserLogin.Name;
                conclusionViewModel.Conclusion = String.IsNullOrEmpty(projectLead.Conclusion) ? "" : projectLead.Conclusion;
                conclusionViewModel.LeadId = projectLead.LeadId;
                conclusionViewModel.Status = projectLead.Status;

                if (conclusionViewModel.Conclusion.Split(':').Length > 1)
                {
                    conclusionViewModel.arrSelectChildStatus = conclusionViewModel.Conclusion.Split(':')[1].Split(',').ToList();

                    List<string> arrSelectChildStatusTrim = new List<string>();
                    foreach (var item in conclusionViewModel.arrSelectChildStatus)
                    {
                        arrSelectChildStatusTrim.Add(item.Trim());
                    }
                    conclusionViewModel.arrSelectChildStatus = arrSelectChildStatusTrim;
                }
                conclusionViewModel.ChildStatus = "";
                if (projectLead.Status > 0)
                {
                    conclusionViewModel.arrChildStatus = leadServices.GetLeadStatus(projectLead.Status).Select(x => x.StatusName).ToList();

                    foreach (string item in conclusionViewModel.arrChildStatus)
                    {
                        if (conclusionViewModel.arrSelectChildStatus != null && conclusionViewModel.arrSelectChildStatus.Contains(item))
                        {
                            conclusionViewModel.ChildStatus += "<input type='checkbox' checked id='" + item + "' name='chkChildStatus' value='" + item + "'>";
                        }
                        else
                        {
                            conclusionViewModel.ChildStatus += "<input type='checkbox' id='" + item + "' name='chkChildStatus' value='" + item + "'>";
                        }
                        conclusionViewModel.ChildStatus += " <label for='" + item + "' >" + item + "</label>";
                        conclusionViewModel.ChildStatus += "<br>";
                    }
                }

                conclusionViewModel.StatusList = leadServices.GetLeadStatus(null).Where(x => x.StatusId != 24).Select(x => new SelectListItem
                {
                    Text = x.StatusName,
                    Value = x.StatusId.ToString()
                }).ToList();



            }
            return PartialView("_ConclusionDetail", conclusionViewModel);
        }

        [HttpPost]
        //[CustomAuthorization(IsAshishTeam: true)]
        public ActionResult Conclusion(ConclusionDto conclusionViewModel, string[] subStatus)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ProjectLead projectlead = new ProjectLead();
                    string joinSubStatus = "";
                    projectlead = leadServices.GetLeadById(conclusionViewModel.LeadId);
                    if (projectlead != null)
                    {
                        LeadTransaction leadTransaction = new LeadTransaction();
                        leadTransaction.LeadId = conclusionViewModel.LeadId;
                        leadTransaction.AddDate = DateTime.Now;
                        leadTransaction.StatusId = conclusionViewModel.Status;
                        joinSubStatus = leadServices.GetLeadStatusById(conclusionViewModel.Status)?.StatusName;
                        //if (subStatus!= null & subStatus.Length > 0)
                        //{
                        //    joinSubStatus += ":";
                        //    joinSubStatus += String.Join(",", subStatus);
                        //}

                        if (!string.IsNullOrEmpty(conclusionViewModel.Conclusion))
                        {
                            if (!conclusionViewModel.Conclusion.Contains(joinSubStatus + ":"))
                            {
                                joinSubStatus += ": " + conclusionViewModel.Conclusion;
                            }
                            else
                            {
                                joinSubStatus = conclusionViewModel.Conclusion;
                            }
                        }

                        leadTransaction.Notes = joinSubStatus;
                        leadTransaction.AddedBy = CurrentUser.Uid;
                        leadServices.SaveLeadTransaction(leadTransaction);
                    }
                    projectlead.Conclusion = joinSubStatus;
                    projectlead.Status = conclusionViewModel.Status;
                    projectlead.ModifyDate = DateTime.Now;
                    projectlead.StatusUpdateDate = conclusionViewModel.StatusUpdateDate.ToDateTime("dd/MM/yyyy").Value;
                    leadServices.SaveLead(projectlead);

                    //ShowSuccessMessage("Success", "Project lead status has been  successfully updated", false);
                    //return Json(new { isSuccess = true, redirectUrl = Url.Action("Index") });

                    return NewtonSoftJsonResult(new RequestOutcome<string>
                    {
                        IsSuccess = true,
                        Message = "Project lead status has been  successfully updated"
                    });
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }


        [HttpGet]
        [CustomAuthorization(IsAshishTeam: true)]
        public JsonResult GetChildStatus(int statusVal)
        {
            var subStatus = leadServices.GetLeadStatus(statusVal).Select(x => x.StatusName).ToList();
            return Json(subStatus);
        }

        [HttpGet]
        //[CustomAuthorization(IsAshishTeam: true)]
        public ActionResult LeadStatus(int id)
        {
            ProjectLead projectLead = new ProjectLead();
            LeadStatusDto leadStatusViewModel = new LeadStatusDto();
            projectLead = leadServices.GetLeadById(id);
            if (projectLead != null)
            {
                leadStatusViewModel.LeadId = projectLead.LeadId;
                leadStatusViewModel.StatusList = leadServices.GetLeadStatus(null).Where(x => x.StatusName.IndexOf("Action") >= 0 || x.StatusName.IndexOf("Chase") >= 0 && x.StatusId != 24).Select(x => new SelectListItem
                {
                    Text = x.StatusName,
                    Value = x.StatusId.ToString()
                }).ToList();
                var AlmostConvertedId = leadServices.GetLeadType("Lead").Where(x => x.TypeName == "Almost Converted").FirstOrDefault().TypeId;
                leadStatusViewModel.LeadType = projectLead.LeadType;
                //ConversionDate = r.ConversionDate.ToFormatDateString("MMM, dd yyyy") ?? "",
                //ShowConversionDate = r.LeadType == AlmostConvertedId,
                if (projectLead.LeadType == AlmostConvertedId)
                {
                    leadStatusViewModel.IsAlmostConverted = true;
                    if (projectLead.ConversionDate != null)
                    {
                        leadStatusViewModel.ConversionDate = projectLead.ConversionDate.ToFormatDateString("dd/MM/yyyy");
                    }
                }
            }
            return PartialView("_LeadStatus", leadStatusViewModel);
        }

        //  [ValidateInput(false)]
        [HttpPost]
        //[CustomAuthorization(IsAshishTeam: true)]
        public ActionResult LeadStatus(LeadStatusDto leadStatusViewModel, IFormFile document)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!IsPM && !leadStatusViewModel.IsConfirmSubmit)
                    {
                        var prList = projectClosureService.GetProjectClosureOnDate(CurrentUser.Uid, leadStatusViewModel.NextChaseDate.ToDateTime("dd/MM/yyyy") ?? DateTime.Now);
                        var plList = leadServices.GetProjectLeadOnDate(CurrentUser.Uid, leadStatusViewModel.NextChaseDate.ToDateTime("dd/MM/yyyy") ?? DateTime.Now);
                        if ((prList.Count() > 0 || plList.Count() > 0))
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "You have already data for same date", Data = (prList.Count() + plList.Count()).ToString() });
                        }
                    }

                    ProjectLead projectLead = leadServices.GetLeadById(leadStatusViewModel.LeadId);
                    if (projectLead != null)
                    {
                        string fileName = GeneralMethods.SaveFile(document, "content/leads", "status-Doc-");
                        LeadTransaction leadTransaction = new LeadTransaction()
                        {
                            LeadId = leadStatusViewModel.LeadId,
                            AddDate = DateTime.Now,
                            AddedBy = CurrentUser.Uid,
                            Doc = fileName,
                            StatusId = leadStatusViewModel.Status,
                            Notes = leadStatusViewModel.Notes,
                            ConversionDate = leadStatusViewModel.ConversionDate == "" ? null : leadStatusViewModel.ConversionDate.ToDateTime("dd/MM/yyyy")
                        };

                        leadServices.SaveLeadTransaction(leadTransaction);
                        projectLead.Status = leadStatusViewModel.Status;
                        projectLead.ModifyDate = DateTime.Now;

                        if (leadStatusViewModel.NextChaseDate.HasValue())
                        {
                            projectLead.NextChasedDate = leadStatusViewModel.NextChaseDate.ToDateTime("dd/MM/yyyy");
                        }
                        if (leadStatusViewModel.ConversionDate.HasValue())
                        {
                            projectLead.ConversionDate = leadStatusViewModel.ConversionDate.ToDateTime("dd/MM/yyyy");
                        }
                        else
                        {
                            projectLead.ConversionDate = null;
                        }
                        switch ((Enums.LeadStatus)leadStatusViewModel.Status)
                        {
                            case Enums.LeadStatus.ChaseRequest:
                                projectLead.ChaseRequests++;
                                break;

                            default:
                                projectLead.ChaseRequests = 0;
                                break;
                        }

                        projectLead.IsUnread = CurrentUser.RoleId == (int)Enums.UserRoles.PM;
                        leadServices.SaveLead(projectLead);

                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "Lead status has been successfully updated"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to get record");
                    }
                }
                else
                {
                    return CreateModelStateErrors();
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }
        }

        [HttpGet]
        //[CustomAuthorization(IsAshishTeam: true)]
        public ActionResult AddEditLead(int? id)
        {
            ViewBag.Industries = domainTypeService.GetDomainList().Select(n => new SelectListItem { Text = n.DomainName, Value = n.DomainId.ToString() }).ToList();
            LeadDto leadViewModel = new LeadDto { EstimateOwnerId = CurrentUser.Uid };
            ProjectLead projectLead = leadServices.GetLeadById(Convert.ToInt32(id));

            var takenTechnicianIds = leadServices.GetTakenUsers();
            var users = userLoginServices.GetUsersByPM(PMUserId);

            leadViewModel.TakenTechnician = users.Where(x => takenTechnicianIds.Contains(x.Uid) &&
                    (x.RoleId != (int)Enums.UserRoles.PM && x.RoleId != (int)Enums.UserRoles.HRBP))
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();

            leadViewModel.TechnicianList = users.Where(x => !takenTechnicianIds.Contains(x.Uid))
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
            
            
            leadViewModel.EstimateOwnerList = users.Where(x => x.RoleId==(int)Enums.UserRoles.PM || RoleValidator.BA_RoleIds.Contains(x.RoleId.Value)
            || RoleValidator.TL_Technical_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_QualityAnalyst_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_DigitalMarketing_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_UIUX_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ARVRUnityGaming_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_Sales_DesignationIds.Contains(x.DesignationId.Value)
            || RoleValidator.TL_ITInfra_DesignationIds.Contains(x.DesignationId.Value)
            //|| RoleValidator.DV_RoleIds.Contains(x.RoleId.Value)
            )
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            var technologylist = new List<string> { "Communication", "Communication Skills" };
            leadViewModel.CommunicatorOwnerList = users.Where(x =>
                (x.User_Tech.Any(s => s.SpecTypeId == (byte)Enums.TechnologySpecializationType.Expert && technologylist.Contains(s.Technology.Title))))
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Uid.ToString()
                }).ToList();

            var takenTechnologyIds = leadServices.TakenTechnologies();
            var technologies = technologyServices.GetTechnologyList().Where(x => x.IsActive == true);

            leadViewModel.TakenTechnologies = technologies.Where(x => takenTechnologyIds.Contains(x.TechId))
                .OrderBy(x => x.Title)
                .Select(x => new SelectListItem { Text = x.Title, Value = x.TechId.ToString() }).ToList();

            leadViewModel.TechnologyList = technologies.Where(x => !takenTechnologyIds.Contains(x.TechId))
                .OrderBy(x => x.Title)
                .Select(x => new SelectListItem { Text = x.Title, Value = x.TechId.ToString() }).ToList();

            SelectListItem otherItem = new SelectListItem()
            {
                Text = "Other",
                Value = "other"
            };

            if (id != null)
            {
                if (projectLead != null)
                {
                    var industries = leadServices.GetProjectLeadIndustry(projectLead.LeadId);
                    if (industries.Count > 0)
                    {
                        leadViewModel.Industry = string.Join(",", industries.Select(x => x.Industry.DomainName).ToList());
                    }
                    leadViewModel.AssignedDate = projectLead.AssignedDate.ToFormatDateString("dd/MM/yyyy");
                    leadViewModel.ChaseRequest = projectLead.ChaseRequests;
                    leadViewModel.CommunicatorOwnerId = projectLead.CommunicatorId;
                    leadViewModel.EstimateOwnerId = projectLead.OwnerId;
                    leadViewModel.EstimateTimeInDays = projectLead.EstimateTimeinDay;
                    leadViewModel.InitialReqDoc = projectLead.InitalRequirement;
                    leadViewModel.IsNewClient = projectLead.IsNewClient;
                    leadViewModel.LeadClientId = projectLead.LeadClientId;
                    leadViewModel.LeadCRMId = projectLead.LeadCRMId;
                    leadViewModel.AbroadPMId = projectLead.AbroadPMID;
                    leadViewModel.LeadId = projectLead.LeadId;
                    leadViewModel.LeadType = projectLead.LeadType;
                    var AlmostConvertedId = leadServices.GetLeadType("Lead").Where(x => x.TypeName == "Almost Converted").FirstOrDefault().TypeId;
                    if (projectLead.LeadType == AlmostConvertedId)
                    {
                        leadViewModel.IsAlmostConverted = true;
                        if (projectLead.ConversionDate != null)
                        {
                            leadViewModel.ConversionDate = projectLead.ConversionDate.ToFormatDateString("dd/MM/yyyy");
                        }
                    }
                    leadViewModel.Notes = projectLead.Notes;
                    leadViewModel.Remark = projectLead.Remark;
                    leadViewModel.Title = projectLead.Title;
                    leadViewModel.ProjectTag = projectLead.Tag;
                    leadViewModel.IsNewClient = projectLead.IsNewClient;
                    leadViewModel.TechnologyOther = projectLead.Technologies;
                    leadViewModel.ProposalDoc = projectLead.ProposalDocument;
                    leadViewModel.OtherDoc = projectLead.OtherDocument;
                    leadViewModel.WireframeMockupsImages = projectLead.Wireframe_MockupsDoc;
                    leadViewModel.WireframeMockupsZip = projectLead.MockupDocument;
                    List<int> leadTechnician = projectLead.LeadTechnicians.Select(T => T.TechnicianId).ToList();
                    leadViewModel.IsCovid19 = projectLead.IsCovid19.HasValue ? projectLead.IsCovid19.Value : false;
                    if (leadTechnician.Count > 0)
                    {
                        foreach (var techni in leadViewModel.TakenTechnician)
                        {
                            if (leadTechnician.Contains(Convert.ToInt32(techni.Value)))
                            {
                                techni.Selected = true;
                            }
                        }
                    }
                    List<int> leadTechnology = projectLead.ProjectLeadTeches.Select(T => T.TechId).ToList();
                    if (leadTechnology.Count > 0)
                    {
                        foreach (var techno in leadViewModel.TakenTechnologies)
                        {
                            if (leadTechnology.Contains(Convert.ToInt32(techno.Value)))
                            {
                                techno.Selected = true;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(leadViewModel.TechnologyOther))
                    {
                        otherItem.Selected = true;
                    }
                }
            }

            leadViewModel.TechnologyList.Add(otherItem);
            foreach (var pms in leadServices.getAbroadPM().GroupBy(x => x.Country))
            {
                SelectListGroup grp = new SelectListGroup() { Name = pms.Key };
                foreach (var pm in pms)
                {
                    leadViewModel.LeadFromList.Add(new SelectListItem
                    {
                        Text = pm.Name,
                        Value = pm.AutoID.ToString(),
                        Group = grp
                    });
                }
            }

            leadViewModel.TypeList = leadServices.GetLeadType("Lead").Select(x => new SelectListItem
            {
                Text = x.TypeName,
                Value = x.TypeId.ToString()
            }).ToList();

            leadViewModel.AssignedDate = DateTime.Now.ToFormatDateString("dd/MM/yyyy");
            return View(leadViewModel);
        }

        //[ValidateInput(false)]
        [HttpPost]
        //[CustomAuthorization(IsAshishTeam: true)]
        public ActionResult AddEditLead(LeadDto leadViewModel)
        {
            var AlmostConvertedId = leadServices.GetLeadType("Lead").Where(x => x.TypeName == "Almost Converted").FirstOrDefault().TypeId;
            if (leadViewModel.LeadType != AlmostConvertedId)
            {
                ModelState.Remove("ConversionDate");
            }

            if (ModelState.IsValid)
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    try
                    {
                        string domainList = null;
                        #region "Uploaded Document Validation"
                        if (leadViewModel.FileProposalDoc != null || leadViewModel.FileRequirmentDoc != null || leadViewModel.FileWireframeMockupsImg != null || leadViewModel.FileWireframeMockupsZip != null || leadViewModel.OtherDoc != null)
                        {
                            int size = 0;
                            string[] requiredExtension;
                            string[] imageRequiredExtension;
                            if (_configuration.GetSection("SiteKeys").GetSection("size") != null && _configuration.GetSection("SiteKeys").GetSection("extension") != null && _configuration.GetSection("SiteKeys").GetSection("ImageExtension") != null)
                            {
                                size = Convert.ToInt32(_configuration.GetSection("SiteKeys").GetSection("size").Value);
                                requiredExtension = _configuration.GetSection("SiteKeys").GetSection("extension").Value.Split(',');
                                imageRequiredExtension = _configuration.GetSection("SiteKeys").GetSection("ImageExtension").Value.Split(',');

                                if (leadViewModel.FileRequirmentDoc != null)
                                {
                                    string fileExt = Path.GetExtension(leadViewModel.FileRequirmentDoc.FileName.ToLower());
                                    if (leadViewModel.FileRequirmentDoc.Length > size)/*Changes by Tabassum*/
                                    {
                                        return Json(new { isSuccess = false, message = "Requirement Document file size must be less then 200 MB" });
                                    }
                                    else if (!requiredExtension.Contains(fileExt))
                                    {
                                        return Json(new { isSuccess = false, message = "Requirement document  extension is not valid" });
                                    }
                                }
                                if (leadViewModel.FileProposalDoc != null)
                                {
                                    string fileExt = Path.GetExtension(leadViewModel.FileProposalDoc.FileName.ToLower());
                                    if (leadViewModel.FileProposalDoc.Length > size)/*Changes by Tabassum*/
                                    {
                                        return Json(new { isSuccess = false, message = "Proposal Document file size must be less then 200 MB" });
                                    }
                                    else if (!requiredExtension.Contains(fileExt))
                                    {
                                        return Json(new { isSuccess = false, message = "Proposal document extension is not valid" });
                                    }
                                }
                                if (leadViewModel.FileWireframeMockupsImg != null)
                                {
                                    string fileExt = Path.GetExtension(leadViewModel.FileWireframeMockupsImg.FileName.ToLower());
                                    if (leadViewModel.FileWireframeMockupsImg.Length > size)/*Changes by Tabassum*/
                                    {
                                        return Json(new { isSuccess = false, message = "Wireframe/Mockups Image file size must be less then 200 MB" });
                                    }
                                    else if (!imageRequiredExtension.Contains(fileExt))
                                    {
                                        return Json(new { isSuccess = false, message = "Wireframe/Mockups Image extension is not valid", updateTargetId = "error-ModalMessage" });
                                    }
                                }
                                if (leadViewModel.FileOtherDoc != null)
                                {
                                    string fileExt = Path.GetExtension(leadViewModel.FileOtherDoc.FileName.ToLower());

                                    string[] allExtension = new string[requiredExtension.Length + imageRequiredExtension.Length];
                                    requiredExtension.CopyTo(allExtension, 0);
                                    imageRequiredExtension.CopyTo(allExtension, requiredExtension.Length);
                                    if (leadViewModel.FileOtherDoc.Length > size)/*Changes by Tabassum*/
                                    {
                                        return Json(new { isSuccess = false, message = "Other Document file size must be less then 200 MB" });
                                    }
                                    else if (!allExtension.Contains(fileExt))
                                    {
                                        return Json(new { isSuccess = false, message = "Other document extension is not valid" });
                                    }
                                }
                                if (leadViewModel.FileWireframeMockupsZip != null)
                                {
                                    string fileExt = Path.GetExtension(leadViewModel.FileWireframeMockupsZip.FileName.ToLower());
                                    if (leadViewModel.FileWireframeMockupsZip.Length > size)/*Changes by Tabassum*/
                                    {
                                        return Json(new { isSuccess = false, message = "Wireframe/Mockups(zip) file size must be less then 200 MB" });
                                    }
                                    else if (!requiredExtension.Contains(fileExt))
                                    {
                                        return Json(new { isSuccess = false, message = "Wireframe/Mockups(zip) extension is not valid" });
                                    }
                                }
                            }
                        }
                        #endregion

                        ProjectLead projectLead = new ProjectLead
                        {
                            Status = (int)Enums.LeadStatus.ActionRequiredFrom_Team,
                            Isdelivered = true,
                            AddDate = DateTime.Now,
                            StatusUpdateDate = DateTime.Now,
                            PMID = PMUserId
                        };
                        if (leadViewModel.LeadId > 0)
                        {
                            projectLead = leadServices.GetLeadById(leadViewModel.LeadId);
                        }
                        else
                        {
                            projectLead.NextChasedDate = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy").ToDateTime(false);
                        }
                        //projectLead.Industry = domainList;
                        projectLead.AbroadPMID = leadViewModel.AbroadPMId;
                        projectLead.AssignedDate = leadViewModel.AssignedDate.ToDateTime("dd/MM/yyyy").Value;
                        projectLead.CommunicatorId = leadViewModel.CommunicatorOwnerId;
                        projectLead.IP = GeneralMethods.Getip();
                        projectLead.IsNewClient = leadViewModel.IsNewClient;
                        projectLead.Title = leadViewModel.Title;
                        //projectLead.TitleCheckSum = leadViewModel.Title.Trim().GetHashCode();
                        //NOTE: To save duplicate Title we are now passing GUID HashCode
                        projectLead.TitleCheckSum = Guid.NewGuid().ToString().GetHashCode();
                        projectLead.OwnerId = leadViewModel.EstimateOwnerId;
                        projectLead.Technologies = !String.IsNullOrEmpty(leadViewModel.TechnologyOther) ? leadViewModel.TechnologyOther : "";
                        projectLead.Tag = leadViewModel.ProjectTag;
                        projectLead.Notes = leadViewModel.Notes;
                        projectLead.LeadType = leadViewModel.LeadType;
                        if (projectLead.LeadType == AlmostConvertedId)
                        {
                            projectLead.ConversionDate = leadViewModel.ConversionDate.ToDateTime("dd/MM/yyyy").Value;
                        }
                        else { projectLead.ConversionDate = null; }

                        projectLead.ModifyDate = DateTime.Now;
                        projectLead.EstimateTimeinDay = leadViewModel.EstimateTimeInDays;
                        projectLead.LeadCRMId = leadViewModel.LeadCRMId;
                        projectLead.Remark = leadViewModel.Remark;
                        projectLead.IsCovid19 = leadViewModel.IsCovid19;

                        leadServices.SaveLead(projectLead);

                        if (leadViewModel.Domains != null && leadViewModel.Domains.Length > 0)
                        {
                            List<ProjectLeadIndustry> projectLeadIndustries = new List<ProjectLeadIndustry>();
                            foreach (var industry in leadViewModel.Domains)
                            {
                                var domain = domainTypeService.GetDomainByName(industry);
                                if (domain != null)
                                {
                                    projectLeadIndustries.Add(new ProjectLeadIndustry { ProjectLeadId = projectLead.LeadId, IndustryId = domain.DomainId });
                                }
                            }
                            leadServices.SaveProjectLeadIndustry(projectLeadIndustries);
                        }

                        List<LeadTechnician> technicianList = new List<LeadTechnician>();
                        foreach (var techni in leadViewModel.Technician)
                        {
                            technicianList.Add(new LeadTechnician { LeadId = projectLead.LeadId, TechnicianId = Convert.ToInt32(techni) });
                        }
                        leadServices.SaveLeadTechnicians(technicianList);
                        List<ProjectLeadTech> technologyList = new List<ProjectLeadTech>();
                        foreach (var tech in leadViewModel.Technology)
                        {
                            if (tech != "other")
                            {
                                technologyList.Add(new ProjectLeadTech() { LeadId = projectLead.LeadId, TechId = Convert.ToInt32(tech) });
                            }
                        }
                        leadServices.SaveLeadTechnology(technologyList);

                        if (leadViewModel.FileRequirmentDoc != null)
                        {
                            projectLead.InitalRequirement = GeneralMethods.SaveFile(leadViewModel.FileRequirmentDoc, "content/leads/", String.Format("Lead_{0}", projectLead.LeadId));
                        }
                        if (leadViewModel.FileProposalDoc != null)
                        {
                            projectLead.ProposalDocument = GeneralMethods.SaveFile(leadViewModel.FileProposalDoc, "Upload/EstimateDocument/", string.Empty);
                        }
                        if (leadViewModel.FileOtherDoc != null)
                        {
                            projectLead.OtherDocument = GeneralMethods.SaveFile(leadViewModel.FileOtherDoc, "Upload/EstimateDocument/", string.Empty);
                        }
                        if (leadViewModel.FileWireframeMockupsImg != null)
                        {
                            projectLead.Wireframe_MockupsDoc = GeneralMethods.SaveFile(leadViewModel.FileWireframeMockupsImg, "Upload/EstimateDocument/", string.Empty);
                        }
                        if (leadViewModel.FileWireframeMockupsZip != null)
                        {
                            projectLead.MockupDocument = GeneralMethods.SaveFile(leadViewModel.FileWireframeMockupsZip, "Upload/EstimateDocument/", string.Empty);
                        }
                        string leadTechnologies = string.Empty;
                        if (!String.IsNullOrEmpty(leadViewModel.TechnologyOther))
                        {
                            leadTechnologies = leadViewModel.TechnologyOther;
                        }
                        if (leadViewModel.Technology.Any())
                        {
                            if (!string.IsNullOrWhiteSpace(leadTechnologies))
                            {
                                List<Technology> technologies = technologyServices.GetTechnologiesByIds
                                    (projectLead.ProjectLeadTeches.Select(x => x.TechId).ToArray());
                                string[] technolohiesName = technologies.Select(x => x.Title).ToArray();
                                leadTechnologies = leadTechnologies + "," + string.Join(",", technolohiesName);
                            }
                            else
                            {
                                List<Technology> technologies = technologyServices.GetTechnologiesByIds(projectLead.ProjectLeadTeches.Select(x => x.TechId).ToArray());
                                string[] technolohiesName = technologies.Select(x => x.Title).ToArray();

                                leadTechnologies = string.Join(",", technolohiesName);
                            }
                        }
                        bool isEstimateExist = projectLead.EstimateDocuments.Any(e => e.LeadId == projectLead.LeadId);
                        if (leadViewModel.FileWireframeMockupsImg != null || leadViewModel.FileWireframeMockupsZip != null || leadViewModel.FileOtherDoc != null || leadViewModel.FileWireframeMockupsZip != null)
                        {
                            if (isEstimateExist)
                            {
                                projectLead.EstimateDocuments.ToList().ForEach(a =>
                                {
                                    a.Title = projectLead.Title;
                                    a.EstimateTimeInDays = projectLead.EstimateTimeinDay;
                                    a.Modified = DateTime.Now;
                                    a.Uid_UploadedBy = CurrentUser.Uid;
                                    a.Technology = leadTechnologies;
                                    a.Tags = projectLead.Tag;
                                    a.DocumentPath = projectLead.ProposalDocument;
                                    a.MockupDocument = projectLead.MockupDocument;
                                    a.OtherDocument = projectLead.OtherDocument;
                                    a.Wireframe_MockupsDoc = projectLead.Wireframe_MockupsDoc;
                                    a.Industry = domainList;
                                });
                            }
                            else
                            {
                                EstimateDocument estimateDocument = new EstimateDocument()
                                {
                                    Created = DateTime.Now,
                                    EstimateTimeInDays = projectLead.EstimateTimeinDay,
                                    Technology = leadTechnologies,
                                    DocumentPath = projectLead.ProposalDocument,
                                    OtherDocument = projectLead.OtherDocument,
                                    Wireframe_MockupsDoc = projectLead.Wireframe_MockupsDoc,
                                    MockupDocument = projectLead.MockupDocument,
                                    LeadId = projectLead.LeadId,
                                    Modified = DateTime.Now,
                                    Tags = projectLead.Tag,
                                    Title = projectLead.Title,
                                    Uid_UploadedBy = CurrentUser.Uid,
                                    Industry = domainList,
                                };
                                leadServices.Save(estimateDocument);
                            }
                        }

                        leadServices.SaveLead(projectLead);
                        ShowSuccessMessage("Success", "Lead has been Successfully added", false);
                        trans.Complete();

                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            RedirectUrl = Url.Action("Index", "Estimate")
                        });
                    }
                    catch (Exception ex)
                    {
                        return MessagePartialView(ex.Message);
                    }
                }
            }
            else
            {
                return CreateModelStateErrors();
            }
        }

        [HttpGet]
        public ActionResult EstimateHistory(int id)
        {
            LeadTransactionDto leadTransactionViewModel = new LeadTransactionDto();
            if (id > 0)
            {
                ProjectLead lead = leadServices.GetLeadById(id);
                if (lead != null)
                {
                    leadTransactionViewModel.LeadId = $"Estimate History (ID # {lead.LeadId})";
                    leadTransactionViewModel.NextChaseDate = (lead.NextChasedDate.HasValue ? $"We will chase Again on:{lead.NextChasedDate.Value.ToFormatDateString("MMM, dd, yyyy")}" : "");
                    leadTransactionViewModel.LeadTransactionList = lead.LeadTransactions.OrderByDescending(o => o.AddDate).Select((x, index) => new TransactionDto
                    {
                        SNo = index + 1,
                        AddedBy = x.UserLogin.Name,
                        AddedDate = x.AddDate.ToFormatDateString("dd/MM/yyyy hh:mm tt"),
                        Document = !string.IsNullOrWhiteSpace(x.Doc) ? SiteKey.DomainName + "content/leads/" + x.Doc : "",
                        DocumentName = !string.IsNullOrWhiteSpace(x.Doc) ? Extensions.TrimLength(x.Doc, 20, true) : "",
                        Status = x.LeadStatu.StatusName,
                        TrasnactionDescrtiption = x.Notes
                    }).ToList();
                }
            }
            return PartialView("_EstimateHistory", leadTransactionViewModel);
        }

        [HttpPost]
        [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult ExportToExcel(string txtSearch, string txtAssignedFrom,
            string txtAssignedTo, string drpOwner, string drpStatus, string drpType, string drpClient,
            string[] chkCountry, string[] chkStatus, bool existClient, bool newClient, bool awaitResp,
            bool escalatedClient, bool newConverted, bool existingConvert, bool isPageLoad, bool isCovid19)
        {
            string Reportname = "leadreport";
            chkStatus = chkStatus != null ? chkStatus.Where(x => x.HasValue()).Select(x => x).ToArray() : null;
            chkCountry = chkCountry != null ? chkCountry.Where(x => x.HasValue()).Select(x => x).ToArray() : null;

            int totalCount;
            List<ProjectLead> projectLeads = FilterProjectLeadDetail(null, txtSearch, txtAssignedFrom,
            txtAssignedTo, drpOwner, drpStatus, drpType, drpClient,
            chkCountry, chkStatus, existClient, newClient, awaitResp,
            escalatedClient, newConverted, existingConvert, isPageLoad, out totalCount, isCovid19: isCovid19);
            if (projectLeads.Count > 0)
            {
                var filename = Reportname.Trim().Replace(" ", "_") + "_" + " printed " + DateTime.Now.ToString("hh_mm_ss") + ".xls";
                var memoryStream = GeneralMethods.ExportToExcelLeads(projectLeads, Reportname, CurrentUser.Uid, CurrentUser.RoleId);
                byte[] filecontent = memoryStream.ToArray();
                return File(filecontent, ExportExcelHelper.ExcelContentType, filename);

                //return File(GeneralMethods.ExportToExcelLeads(projectLeads, Reportname, CurrentUser.Uid, CurrentUser.RoleId), "application/vnd.ms-excel", Reportname + " printed " + DateTime.Now.ToString("hh_mm_ss") + ".xls");
            }

            return Content("No Record found");
        }

        [HttpGet]
        [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult DeleteLead()
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Lead?",
                Header = new ModalHeader { Heading = "Delete Lead" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No", DefaultButtonCss = true }
            });
        }

        [HttpPost]
        [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult DeleteLead(int id)
        {

            try
            {
                leadServices.DeleteLead(id);
                ShowSuccessMessage("Success!", "Lead has been successfully deleted", false);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error!", ex.GetBaseException().Message, false);
                return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("Index", "Estimate") });
            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, RedirectUrl = Url.Action("Index", "Estimate") });
        }

        private string WeekAndDay(int noofday)
        {
            int weeks = noofday / 5;
            int days = noofday % 5;
            string result = ""; ;
            string weektypes = "Weeks";
            string daytypes = "Days";

            if (days == 1) { daytypes = "Day"; }
            if (weeks == 1) { weektypes = "Week"; }

            if (days == 0) { daytypes = ""; }
            if (weeks == 0) { weektypes = ""; }

            if (weeks == 0)
                result = days + " " + daytypes;
            if (days == 0)
                result = weeks.ToString() + " " + weektypes;
            if (weeks != 0 && days != 0)
                result = weeks.ToString() + " " + weektypes + "  " + days + " " + daytypes;
            if (weeks == 0 && days == 0)
                result = "";

            return result;
        }

        [HttpPost]
        [CustomAuthorization(IsAshishTeam: true)]
        public ActionResult DeleteDocument(int id, string documentId)
        {
            var leadEntity = leadServices.GetLeadById(id);
            string message = "";
            bool isSuccess = false;
            switch (documentId)
            {
                case "lnkDelProposalDoc":
                    if (!String.IsNullOrEmpty(leadEntity.Wireframe_MockupsDoc) || !String.IsNullOrEmpty(leadEntity.MockupDocument) || !String.IsNullOrEmpty(leadEntity.OtherDocument))
                    {
                        leadEntity.ProposalDocument = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    break;

                case "lnkDelWireframeMockupDoc":
                    if (!String.IsNullOrEmpty(leadEntity.ProposalDocument) || !String.IsNullOrEmpty(leadEntity.MockupDocument) || String.IsNullOrEmpty(leadEntity.OtherDocument))
                    {
                        leadEntity.Wireframe_MockupsDoc = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    message = "File can not be deleted";
                    break;

                case "lnkDelOtherDoc":
                    if (!String.IsNullOrEmpty(leadEntity.ProposalDocument) || !String.IsNullOrEmpty(leadEntity.Wireframe_MockupsDoc) || !String.IsNullOrEmpty(leadEntity.MockupDocument))
                    {
                        leadEntity.OtherDocument = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    break;

                case "lnkDelMockupDoc":
                    if (!String.IsNullOrEmpty(leadEntity.ProposalDocument) || !String.IsNullOrEmpty(leadEntity.Wireframe_MockupsDoc) || !String.IsNullOrEmpty(leadEntity.OtherDocument))
                    {
                        leadEntity.MockupDocument = string.Empty;
                        message = "File has been successfully deleted";
                        isSuccess = true;
                    }
                    else
                    {
                        message = "File can not be deleted. Atleast one file required";
                        isSuccess = false;
                    }
                    break;
            }
            leadServices.SaveLead(leadEntity);

            return Json(new { isSuccess, message });
        }

        [HttpGet]
        public ActionResult LeadNotes(int id)
        {
            //id = 7708;
            LeadNotesDto leadNotesViewModel = new LeadNotesDto();
            if (id > 0)
            {
                leadNotesViewModel.LeadId = $"CRM Lead Notes (ID # {id})";

                var response = GetLeadNotes(id);
                string responseData = "";

                List<LeadNotesJson> leadNotes = new List<LeadNotesJson>();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    responseData = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    if (responseData.HasValue())
                    {
                        if (responseData != null)
                        {
                            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(responseData);
                            if (Convert.ToBoolean(obj["Status"]))
                            {
                                String json = null;
                                //json = JsonConvert.SerializeObject(obj.SelectToken("Data.notesInfo.response.result"));
                                json = JsonConvert.SerializeObject(obj.SelectToken("Data.response"));
                                leadNotes = JsonConvert.DeserializeObject<List<LeadNotesJson>>(json);

                                leadNotesViewModel.LeadNoteList = leadNotes.Select((x, index) => new LeadNote
                                {
                                    SNo = index + 1,
                                    lead_id = x.lead_id,
                                    notes_details = x.notes_details,
                                    //notes_time = x.notes_time,
                                    notes_time = UnixTimeStampToDateTime(Convert.ToDouble(x.notes_time)).ToFormatDateString("MMM dd yyyy hh:mm tt"),
                                    user_name = x.user_name,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            return PartialView("_LeadNotes", leadNotesViewModel);
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        private HttpWebResponse GetLeadNotes(int lead_id)
        {
            var request = WebRequest.CreateHttp(SiteKey.CRMApiLeadNotesUrl);
            StringBuilder postData = new StringBuilder();
            postData.Append($"lead_id={lead_id}&");

            request.Headers.Add("userid", SiteKey.CRMApiUser);
            request.Headers.Add("password", SiteKey.CRMApiPassword);

            var data = Encoding.ASCII.GetBytes(postData.ToString());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var result = (HttpWebResponse)(request.GetResponse());
            return result;
        }


        #region "Role wise Calculation"

        [HttpGet]
        public IActionResult SearchPriceCalculation(string crmleadid, int estimatemodelid, string estimateName)
        {
            ViewBag.EstimateRole = estimateService.GetEstimateRoleDropdown();
            ViewBag.TechnologyParent = estimateService.GetEstimateTechnologyItemList();
            ViewBag.EstimateModel = estimateService.GetEstimateModelSelectList();
            //ViewBag.Country = estimateService.GetCountrySelectList();

            var priceCalculationList = estimateService.GetEstimatePriceCalculationByCRMUserId(crmleadid, CurrentUser.RoleId, CurrentUser.PMUid);

            var priceCalculation = priceCalculationList.FirstOrDefault(x => (estimatemodelid == 0 || x.EstimateModelId == estimatemodelid) && (string.IsNullOrEmpty(estimateName) || x.EstimateName == estimateName));

            EstimateCalculationDto model = new EstimateCalculationDto();
            var estimateFormList = new List<EstimateFormDto>();
            if (priceCalculation != null)
            {
                model.CRMLeadId = crmleadid;
                model.EstimateModelId = priceCalculation.EstimateModelId;
                model.EstimateName = priceCalculation.EstimateName;
                ModelState["EstimateModelId"].RawValue = priceCalculation.EstimateModelId;
                ModelState["EstimateName"].RawValue = priceCalculation.EstimateName;
                //model.CountryId = priceCalculation.CountryId;
                foreach (var item in priceCalculation.EstimatePriceCalculationDetail)
                {
                    var estimateForm = new EstimateFormDto();
                    estimateForm.RoleId = item.EstimateRoleId;
                    estimateForm.RoleName = item.EstimateRole.Name;
                    if (item.EstimateTechnologyId.HasValue)
                    {
                        estimateForm.TechnologyId = item.EstimateTechnologyId.Value;
                        estimateForm.TechnologyName = item.EstimateTechnology.Title;
                    }
                    estimateForm.ExperienceId = item.EstimateRoleExpId;
                    estimateForm.ExperienceName = item.EstimateRoleExp.Name;
                    estimateForm.NoOfResources = item.NoOfResources;
                    estimateForm.EstimateHour = item.EstimateHours;
                    estimateForm.MinPrice = item.Price;
                    estimateForm.Price = item.Price;
                    var estimateRoleExp = estimateService.GetEstimateRoleExpDropdown(estimateForm.RoleId);
                    estimateForm.EstimateRoleExpList = estimateRoleExp;
                    estimateFormList.Add(estimateForm);
                }

                model.EstimateForm = estimateFormList;
                CalculationResult(model);
                GetEstimateHostingPackage(model);

                model.EstimateModel = priceCalculationList.Select(item => new EstimateModelDto
                {
                    Id = item.EstimateModelId,
                    Name = item.EstimateModel?.Name,
                    EstimateName = item.EstimateName,
                }).ToList();
            }
            else
            {
                string msg = string.Empty;
                var isCrmExist = estimateService.IsEstimationPriceExists(crmleadid, estimatemodelid, estimateName);
                if (isCrmExist)
                {
                    msg = $"You don't have permission to access this lead id ({crmleadid})";
                }
                else
                {
                    msg = $"No records saved against this {crmleadid} lead";
                }

                return Ok(new { Status = false, Message = msg });
            }

            //return PartialView("_EstimateForm", modelDetail);
            return PartialView("_PriceCalculationForm", model);
        }


        [HttpGet]
        public IActionResult PriceCalculation()
        {
            TempData["CountryId"] = null;
            EstimateCalculationDto model = null;
            model = new EstimateCalculationDto();
            model.EstimateForm.Add(new EstimateFormDto());
            ViewBag.EstimateRole = estimateService.GetEstimateRoleDropdown();
            ViewBag.TechnologyParent = estimateService.GetEstimateTechnologyItemList();
            ViewBag.EstimateModel = estimateService.GetEstimateModelSelectList();
            //ViewBag.Country = estimateService.GetCountrySelectList();

            return View(model);
        }


        [HttpPost]
        public IActionResult PriceCalculation(EstimateCalculationDto model)
        {
            if (model.Command == "Submit")
            {
                model.CRMLeadId = string.Empty;
                model.EstimateModelId = null;
                model.EstimateName = string.Empty;
                ModelState["EstimateModelId"].RawValue = null;
                ModelState["EstimateName"].RawValue = string.Empty;
                FormCalculation(model);

                return View(model);

            }
            else
            {
                bool issave = true;
                var priceCalculation = estimateService.GetEstimatePriceCalculation(model.CRMLeadId, model.EstimateModelId ?? 0, model.EstimateName);
                if (priceCalculation != null && priceCalculation.CreatedByUid != null && (priceCalculation.CreatedByUid != CurrentUser.Uid && priceCalculation.CreatedByU.PMUid != CurrentUser.Uid && priceCalculation.CreatedByU.TLId != CurrentUser.Uid))
                {
                    issave = false;
                }

                if (issave)
                {
                    if (model.IsOverWrite && !string.IsNullOrEmpty(model.CRMLeadId) && model.EstimateModelId > 0)
                    {
                        estimateService.DeleteEstimatePriceCalculation(model.CRMLeadId, model.EstimateModelId ?? 0, model.EstimateName);
                    }

                    var usdCurrency = currencyService.GetCurrencyByName("USD");
                    var audCurrency = currencyService.GetCurrencyByName("AUD");
                    var aedCurrency = currencyService.GetCurrencyByName("AED");
                    var totalCost = model.EstimateForm.Sum(x => ((x.EstimateHour * x.NoOfResources) * x.Price));


                    if (priceCalculation == null)
                    {
                        priceCalculation = new EstimatePriceCalculation();
                        priceCalculation.CrmleadId = model.CRMLeadId;
                        priceCalculation.EstimateModelId = model.EstimateModelId;
                        priceCalculation.CreatedByUid = CurrentUser.Uid;
                        priceCalculation.ModifiedByUid = CurrentUser.Uid;
                        priceCalculation.EstimateName = model.EstimateName.Trim();

                        priceCalculation.TotalCost = totalCost;
                        priceCalculation.ExchangeRateUsd = usdCurrency.ExchangeRate;
                        priceCalculation.ExchangeRateAud = audCurrency.ExchangeRate;
                        priceCalculation.CountryId = model.CountryId;
                    }
                    else
                    {
                        priceCalculation.TotalCost = totalCost;
                        priceCalculation.ExchangeRateUsd = usdCurrency.ExchangeRate;
                        priceCalculation.ExchangeRateAud = audCurrency.ExchangeRate;
                        priceCalculation.ModifiedByUid = CurrentUser.Uid;
                        if(priceCalculation.CreatedByUid == null)
                        {
                            priceCalculation.CreatedByUid = CurrentUser.Uid;
                        }
                    }

                    if (model.EstimateForm.Count > 0)
                    {
                        foreach (var item in model.EstimateForm)
                        {
                            EstimatePriceCalculationDetail priceCalculationDetail = new EstimatePriceCalculationDetail();

                            priceCalculationDetail.EstimateRoleId = item.RoleId;
                            if (item.TechnologyId > 0)
                            {
                                priceCalculationDetail.EstimateTechnologyId = item.TechnologyId;
                            }
                            priceCalculationDetail.EstimateRoleExpId = item.ExperienceId;
                            priceCalculationDetail.NoOfResources = item.NoOfResources;
                            priceCalculationDetail.EstimateHours = Convert.ToInt32(item.EstimateHour);
                            priceCalculationDetail.Price = item.Price;
                            priceCalculationDetail.CreatedDate = DateTime.Now;
                            priceCalculationDetail.ModifiedDate = DateTime.Now;
                            priceCalculation.EstimatePriceCalculationDetail.Add(priceCalculationDetail);
                        }
                    }

                    estimateService.Save(priceCalculation);
                    ShowSuccessMessage("Success", "Estimate saved successfully", false);
                }
                else
                {
                    ShowErrorMessage("Error", "You don't have permission to change this lead id ", false);

                    FormCalculation(model);
                    return View(model);
                }
            }

            return RedirectToAction("PriceCalculation", "Estimate");

        }

        private void FormCalculation(EstimateCalculationDto model)
        {
            if (model.EstimateForm.Count > 0)
            {
                CalculationResult(model);
                GetEstimateHostingPackage(model);

                //TempData.Put("EstimateCalculation", model);

                foreach (var item in model.EstimateForm)
                {
                    var estimateRoleExp = estimateService.GetEstimateRoleExpDropdown(item.RoleId);
                    item.EstimateRoleExpList = estimateRoleExp;
                }
            }
            else
            {
                model.EstimateForm.Add(new EstimateFormDto());
            }

            ViewBag.EstimateRole = estimateService.GetEstimateRoleDropdown();
            ViewBag.TechnologyParent = estimateService.GetEstimateTechnologyItemList();
            ViewBag.EstimateModel = estimateService.GetEstimateModelSelectList();
            //ViewBag.Country = estimateService.GetCountrySelectList();
        }

        [HttpPost]
        public IActionResult CheckCRMLeadExist(string crmLeadId, int estimatemodelid, string estimateName)
        {
            var isCrmExist = estimateService.IsEstimationPriceExists(crmLeadId, estimatemodelid, estimateName);
            return Json(new { IsCrmExist = isCrmExist });
        }


        public IActionResult ExportCalculation()
        {
            var model = TempData.Get<EstimateCalculationDto>("EstimateCalculation");
            if (model != null)
                TempData.Put("EstimateCalculation", model);

            DataTable table = new DataTable("EstimateCalculation");
            table.Columns.Add("Role");
            table.Columns.Add("Experience (Years)");
            table.Columns.Add("Per Hour Cost (£)");
            table.Columns.Add("No. of Resources");
            table.Columns.Add("Per Resource Hours");
            table.Columns.Add("Total Estimated Hours");
            table.Columns.Add("Total Cost (£)");
            table.Columns.Add("Total Cost (USD)");
            table.Columns.Add("Total Cost (AUD)");
            table.Columns.Add("Team composition for estimate");

            var usdCurrency = currencyService.GetCurrencyByName("USD");
            var audCurrency = currencyService.GetCurrencyByName("AUD");
            foreach (var item in model.EstimateForm)
            {
                var totalCost = (item.Price * (item.EstimateHour * item.NoOfResources));
                DataRow dr = table.NewRow();
                dr["Role"] = $"{item.RoleName} {(string.IsNullOrEmpty(item.TechnologyName) ? string.Empty : $" ({item.TechnologyName})")}";
                dr["Experience (Years)"] = item.ExperienceName;
                dr["Per Hour Cost (£)"] = item.Price;
                dr["No. of Resources"] = item.NoOfResources;
                dr["Per Resource Hours"] = item.EstimateHour;
                dr["Total Estimated Hours"] = (item.EstimateHour * item.NoOfResources);
                dr["Total Cost (£)"] = totalCost.ToString("0.00");
                dr["Total Cost (USD)"] = ((totalCost * Convert.ToDecimal(usdCurrency.ExchangeRate))).ToString("0.00");
                dr["Total Cost (AUD)"] = (totalCost * Convert.ToDecimal(audCurrency.ExchangeRate)).ToString("0.00");
                dr["Team composition for estimate"] = $"{item.NoOfResources} {item.TechnologyName} {(item.NoOfResources > 1 ? $"{item.RoleName}(s)" : item.RoleName)}";
                table.Rows.Add(dr);
            }

            //for (int i = 0; i < 2; i++)
            //{
            //    DataRow dr = table.NewRow();
            //    dr["Role"] = "";
            //    dr["Experience (Years)"] = "";
            //    dr["Per Hour Cost (£)"] = "";
            //    dr["No. of Resources"] = "";
            //    dr["Per Resource Hours"] = "";
            //    dr["Total Estimated Hours"] ="";
            //    dr["Total Cost (£)"] = "";
            //    dr["Total Cost (USD)"] = "";
            //    dr["Total Cost (AUD)"] = "";
            //    dr["Team composition for estimate"] = "";
            //    table.Rows.Add(dr);
            //}

            //foreach (var item in model.CalculationResult)
            //{
            //    DataRow dr = table.NewRow();
            //    dr["Role"] = item.Heading;
            //    dr["Experience (Years)"] = item.Pound;
            //    dr["Per Hour Cost (£)"] = item.USD;
            //    dr["No. of Resources"] = item.AUD;
            //    dr["Per Resource Hours"] = "";
            //    dr["Total Estimated Hours"] = "";
            //    dr["Total Cost (£)"] = "";
            //    dr["Total Cost (USD)"] = "";
            //    dr["Total Cost (AUD)"] = "";
            //    dr["Team composition for estimate"] = "";
            //    table.Rows.Add(dr);
            //}

            //DataRow dr1 = table.NewRow();
            //dr1["Role"] = "Team composition for estimate";
            //dr1["Experience (Years)"] = (string.Join(", ", model.EstimateForm.GroupBy(x => new { x.RoleId, x.TechnologyId }).Select(x => $"{x.Sum(s => s.NoOfResources)} {x.FirstOrDefault().TechnologyName} {x.FirstOrDefault().RoleName}{(x.Sum(s => s.NoOfResources) > 1 ? "(s)" : "")}")));
            //dr1["Per Hour Cost (£)"] = "";
            //dr1["No. of Resources"] = "";
            //dr1["Per Resource Hours"] = "";
            //dr1["Total Estimated Hours"] = "";
            //dr1["Total Cost (£)"] = "";
            //dr1["Total Cost (USD)"] = "";
            //dr1["Total Cost (AUD)"] = "";
            //dr1["Team composition for estimate"] = "";
            //table.Rows.Add(dr1);

            string filename = "EstimateCalculation_" + DateTime.Now.Ticks.ToString() + ".xlsx";
            string[] columns = { "Role", "Experience (Years)", "Per Hour Cost (£)", "No. of Resources", "Per Resource Hours", "Total Estimated Hours", "Total Cost (£)", "Total Cost (USD)", "Total Cost (AUD)", "Team composition for estimate" };
            byte[] filecontent = ExportExcelHelper.ExportExcel(table, "Estimate Calculation Report ", false, columns);
            string fileName = filename;
            return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
        }

        [HttpGet]
        public IActionResult ExportToExcel()
        {
            var model = TempData.Get<EstimateCalculationDto>("EstimateCalculation");
            if (model != null)
                TempData.Put("EstimateCalculation", model);

            if (model != null)
            {
                string filename = "EstimateCalculation_" + DateTime.Now.Ticks.ToString() + ".xlsx";
                var memoryStream = ExportToExcel(model, "Estimate Calculation Report");

                return File(memoryStream, "application/vnd.ms-excel", filename);
            }
            return Json(new { Status = false, Message = "" });
        }

        public IActionResult AddEstimateForm()
        {
            ViewBag.EstimateRole = estimateService.GetEstimateRoleDropdown();
            ViewBag.TechnologyParent = estimateService.GetEstimateTechnologyItemList();// technologyParentService.GetTechnologyParentDropdown();
            var modelDetail = new List<EstimateFormDto>();
            modelDetail.Add(new EstimateFormDto());
            return PartialView("_EstimateForm", modelDetail);
        }

        public IActionResult EstimateRoleExp(int estimateRoleId)
        {
            var data = estimateService.GetEstimateRoleExpDropdown(estimateRoleId);
            return Ok(data);
        }

        public IActionResult EstimatePrice(int roleId, int estimateRoleExpId, int? technologyParentId)
        {
            decimal price = 0;
            decimal minprice = 0;
            var data = estimateService.GetEstimateRoleTechnoloyPrice(roleId, estimateRoleExpId, technologyParentId);
            if (data != null)
            {
                price = data.Price;
            }

            var mindata = estimateService.GetEstimateRoleTechnoloyPrice(roleId, technologyParentId);
            if (mindata != null)
            {
                minprice = mindata.Price;
            }
            return Ok(new { Price = price, MinPrice = minprice });
        }

        [HttpPost]
        public IActionResult GetServerHostingPackage(int countryId, List<int> technologies)
        {
            var estimateHostingPackage = estimateHostingPackageService.GetByTechnologies(technologies, countryId);
            var estimateHostingPackageDtoList = new List<EstimateHostingPackageDto>();
            if (estimateHostingPackage.Count > 0)
            {
                foreach (var item in estimateHostingPackage)
                {
                    estimateHostingPackageDtoList.Add(new EstimateHostingPackageDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        PackageDetail = item.PackageDetail,
                        CurrSign = item.Currency?.CurrSign,
                        HostingCost = item.HostingCost,
                        HostingCostTypeName = ((EstimateCostType)item.HostingCostType).GetDescription(),
                        HostingCostType = item.HostingCostType,
                        SetupCost = item.SetupCost,
                        SetupCostTypeName = ((EstimateCostType)item.SetupCostType).GetDescription(),
                        SetupCostType = item.SetupCostType,
                        CountryName = item.Country.Name,
                        CountryId = item.CountryId,
                        CurrencyId = item.CurrencyId,
                        TechnologyName = string.Join(", ", item.EstimateHostingPackageTechnology.Where(x => technologies.Any(a => a == x.EstimateTechnologyId)).Select(x => x.EstimateTechnology.Title)),
                    });
                }
            }

            return PartialView("_EstimateHostingPackageList", estimateHostingPackageDtoList);
        }

        private void CalculationResult(EstimateCalculationDto model)
        {
            var usdCurrency = currencyService.GetCurrencyByName("USD");
            var audCurrency = currencyService.GetCurrencyByName("AUD");
            var aedCurrency = currencyService.GetCurrencyByName("AED");

            var totalEstimateHour = model.EstimateForm.Sum(x => (x.EstimateHour * x.NoOfResources));
            model.CalculationResult.Add(new CalculationResultDto
            {
                Heading = "Total Estimated Time",
                Pound = totalEstimateHour.ToString(),
                USD = totalEstimateHour.ToString(),
                AUD = totalEstimateHour.ToString(),
                AED = totalEstimateHour.ToString(),
            });

            var roleGroup = model.EstimateForm.GroupBy(x => x.RoleId);
            foreach (var grup in roleGroup)
            {
                var estimateRole = estimateService.GetEstimateRole(grup.Key);
                //var estimateRoleExp = estimateService.GetEstimateRoleExp(grup.FirstOrDefault().ExperienceId);
                var resourcesHour1 = grup.Sum(x => (x.EstimateHour * x.NoOfResources));
                //var minPrice1 = grup.Sum(x => ((x.EstimateHour * x.NoOfResources) * x.Price));
                //if (minPrice1 > 0)
                //{
                //    minPrice1 = minPrice1 * 90 / 100;
                //}
                if (estimateRole.Name == "Developer")
                {
                    estimateRole.Name = "Development";
                }
                model.CalculationResult.Add(new CalculationResultDto
                {
                    Heading = estimateRole.Name + " Hours ",
                    Pound = resourcesHour1.ToString(),
                    USD = resourcesHour1.ToString(),
                    AUD = resourcesHour1.ToString(),
                    AED = resourcesHour1.ToString(),
                    //MinPound = minPrice1.ToString(),
                    //MinUSD = (minPrice1 * Convert.ToDecimal(usdCurrency.ExchangeRate)).ToString(),
                    //MinAUD = (minPrice1 * Convert.ToDecimal(audCurrency.ExchangeRate)).ToString(),
                    IsBasePrice = false,
                    IsCollapsePrice = estimateRole.IsSdlc ?? false
                });
                model.EstimateGraph.Add(new EstimateGraphDto
                {
                    Name = grup.FirstOrDefault().RoleName,
                    Y = resourcesHour1,
                    Color = "#925203"
                });
            }

            decimal percent = 0;
            var resourcesHour = model.CalculationResult.Where(x => x.IsCollapsePrice == true).Sum(x => Convert.ToDecimal(x.Pound));
            if (resourcesHour > 0 && totalEstimateHour > 0)
            {
                percent = resourcesHour / totalEstimateHour * 100;
            }

            model.CalculationResult.Add(new CalculationResultDto
            {
                Heading = "SDLC Time",
                Pound = resourcesHour.ToString(),
                USD = resourcesHour.ToString(),
                AUD = resourcesHour.ToString(),
                AED = resourcesHour.ToString(),
                Percent = percent,
            });

            model.TotalDurationOfProject = totalEstimateHour - resourcesHour + " Hours";

            var pound = model.EstimateForm.Sum(x => ((x.EstimateHour * x.NoOfResources) * x.Price));
            //minPrice = model.EstimateForm.Sum(x => ((x.EstimateHour * x.NoOfResources) * x.MinPrice));
            var minPrice = pound * 90 / 100;
            var usd = (pound * Convert.ToDecimal(usdCurrency.ExchangeRate));
            var aud = (pound * Convert.ToDecimal(audCurrency.ExchangeRate));
            //var aed = (pound * Convert.ToDecimal(aedCurrency.ExchangeRate));
            var aed = Math.Truncate(Math.Truncate((pound * Convert.ToDecimal(aedCurrency.ExchangeRate))) / 10) * 10;
            var minusd = (minPrice * Convert.ToDecimal(usdCurrency.ExchangeRate));
            var minaud = (minPrice * Convert.ToDecimal(audCurrency.ExchangeRate));
            model.CalculationResult.Add(new CalculationResultDto
            {
                Heading = "Total Project Cost",
                Pound = $"£ {(pound).ToString("0.00")}",
                USD = $"USD {(usd).ToString("0.00")}",
                AUD = $"AUD {(aud).ToString("0.00")}",
                AED = $"AED {(aed).ToString("0.00")}",
                //MinPound = minPrice.ToString("0.00"),
                //MinUSD = (minusd).ToString("0.00"),
                //MinAUD = (minaud).ToString("0.00"),
                IsBasePrice = true,
            });

            model.CalculationResult.Add(new CalculationResultDto
            {
                Heading = "Per Hour Project Cost",
                Pound = $"£ {(pound > 0 && totalEstimateHour > 0 ? (pound / totalEstimateHour) : 0).ToString("0.00")}",
                USD = $"USD {(usd > 0 && totalEstimateHour > 0 ? (usd / totalEstimateHour) : 0).ToString("0.00")}",
                AUD = $"AUD {(aud > 0 && totalEstimateHour > 0 ? (aud / totalEstimateHour) : 0).ToString("0.00")}",
                AED = $"AED {(aed > 0 && totalEstimateHour > 0 ? (aed / totalEstimateHour) : 0).ToString("0.00")}",
                //MinPound = (minPrice / totalEstimateHour).ToString("0.00"),
                //MinUSD = (minusd / totalEstimateHour).ToString("0.00"),
                //MinAUD = (minaud / totalEstimateHour).ToString("0.00"),
                IsBasePrice = true,
            });
        }

        private void GetEstimateHostingPackage(EstimateCalculationDto model)
        {
            var technologies = model.EstimateForm.Where(x => x.TechnologyId > 0).Select(x => x.TechnologyId).ToList();
            var country = estimateHostingPackageService.GetByCountryByTechnologies(technologies);
            model.UKCount = country.Count(x => x.Id == 2);
            model.USCount = country.Count(x => x.Id == 3);
            model.AUSCount = country.Count(x => x.Id == 4);
            model.UAECount = country.Count(x => x.Id == 5);

            //var estimateHostingPackage = estimateHostingPackageService.GetByTechnologies(technologies);

            //var gentity = estimateHostingPackage.GroupBy(x => x.CountryId);

            //model.UKCount = gentity.Where(x => x.Key == 2).Count();
            //model.USCount = gentity.Where(x => x.Key == 3).Count();
            //model.AUSCount = gentity.Where(x => x.Key == 4).Count();

            //model.EstimateHostingPackage = new List<EstimateHostingPackageDto>();
            //if (estimateHostingPackage.Count > 0)
            //{
            //    foreach (var item in estimateHostingPackage)
            //    {
            //        model.EstimateHostingPackage.Add(new EstimateHostingPackageDto
            //        {
            //            Id = item.Id,
            //            Name = item.Name,
            //            PackageDetail = item.PackageDetail,
            //            CurrSign = item.Currency?.CurrSign,
            //            HostingCost = item.HostingCost,
            //            HostingCostTypeName = ((EstimateCostType)item.HostingCostType).GetDescription(),
            //            HostingCostType = item.HostingCostType,
            //            SetupCost = item.SetupCost,
            //            SetupCostTypeName = ((EstimateCostType)item.SetupCostType).GetDescription(),
            //            SetupCostType = item.SetupCostType,
            //            CountryName = item.Country.Name,
            //            CountryId = item.CountryId,
            //            CurrencyId = item.CurrencyId,
            //        });
            //    }
            //}
        }

        private MemoryStream ExportToExcel(EstimateCalculationDto source, string reportname)
        {
            MemoryStream response = new MemoryStream();
            if (source.EstimateForm.Any())
            {
                var workbook = new HSSFWorkbook();
                var headerLabelCellStyle = workbook.CreateCellStyle();
                headerLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                headerLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                headerLabelCellStyle.BorderBottom = CellBorderType.THIN;
                headerLabelCellStyle.WrapText = true;

                var headerLabelFont = workbook.CreateFont();
                headerLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                headerLabelFont.Color = HSSFColor.BLACK.index;
                headerLabelCellStyle.SetFont(headerLabelFont);

                var headerCellStyle = workbook.CreateCellStyle();
                headerCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                headerLabelFont.FontHeightInPoints = 11;
                headerCellStyle.SetFont(headerLabelFont);


                var InfoLabelCellStyle = workbook.CreateCellStyle();

                InfoLabelCellStyle.Alignment = HorizontalAlignment.CENTER;
                InfoLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                //InfoLabelCellStyle.BorderBottom = CellBorderType.THIN;
                InfoLabelCellStyle.WrapText = false;
                InfoLabelCellStyle.FillForegroundColor = HSSFColor.YELLOW.index;
                InfoLabelCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var infoLabelFont = workbook.CreateFont();
                infoLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                infoLabelFont.Color = HSSFColor.BLACK.index;
                infoLabelFont.FontHeightInPoints = 14;
                InfoLabelCellStyle.SetFont(infoLabelFont);

                var sheet = workbook.CreateSheet(reportname);
                var attendeeLabelCellStyle = workbook.CreateCellStyle();

                attendeeLabelCellStyle.Alignment = HorizontalAlignment.LEFT;
                attendeeLabelCellStyle.VerticalAlignment = VerticalAlignment.CENTER;
                attendeeLabelCellStyle.BorderBottom = CellBorderType.THIN;
                attendeeLabelCellStyle.WrapText = true;
                attendeeLabelCellStyle.FillForegroundColor = HSSFColor.GREY_50_PERCENT.index;
                attendeeLabelCellStyle.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var attendeeLabelFont = workbook.CreateFont();
                attendeeLabelFont.Boldweight = (short)FontBoldWeight.BOLD;
                attendeeLabelFont.Color = HSSFColor.WHITE.index;
                attendeeLabelCellStyle.SetFont(attendeeLabelFont);

                sheet.CreateRow(0);
                var headerInfo = sheet.CreateRow(1);
                headerInfo.CreateCell(0, CellType.STRING).SetCellValue("Estimate Calculation Report");
                headerInfo.GetCell(0).CellStyle = InfoLabelCellStyle;
                NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(1, 1, 0, 2);
                sheet.AddMergedRegion(cra);
                sheet.CreateRow(2);
                //Create a header row
                var headerRow = sheet.CreateRow(3);
                int counter = 0;
                //Set the column names in the header row 
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Role");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Experience (Years)");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Per Hour Cost (£)");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("No. of Resources");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Per Resource Hours");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Total Estimated Hours");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Total Cost (£)");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Total Cost (USD)");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Total Cost (AUD)");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Total Cost (AED)");
                headerRow.CreateCell(counter++, CellType.STRING).SetCellValue("Team composition for estimate");
                counter = 0;

                for (int i = 0; i < headerRow.LastCellNum; i++)
                {
                    headerRow.GetCell(i).CellStyle = attendeeLabelCellStyle;
                    //headerRow.Height = 600;
                }

                //(Optional) freeze the header row so it is not scrolled
                //sheet.CreateFreezePane(0, 4, 0, 4);

                int rowNumber = 4;

                var rowRedCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                rowRedCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;
                rowRedCellStyleattendeeothercolumn.BorderTop = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderBottom = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderLeft = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.BorderRight = CellBorderType.THIN;
                rowRedCellStyleattendeeothercolumn.FillPattern = FillPatternType.SOLID_FOREGROUND;
                var otherattendeeLabelFont = workbook.CreateFont();
                otherattendeeLabelFont.FontHeightInPoints = 9;
                rowRedCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);

                var usdCurrency = currencyService.GetCurrencyByName("USD");
                var audCurrency = currencyService.GetCurrencyByName("AUD");
                var uaeCurrency = currencyService.GetCurrencyByName("AED");
                for (int i = 0; i < source.EstimateForm.Count(); i++)
                {
                    var row = sheet.CreateRow(rowNumber++);
                    var item = source.EstimateForm[i];
                    var totalCost = (item.Price * (item.EstimateHour * item.NoOfResources));
                    row.CreateCell(counter++).SetCellValue($"{item.RoleName} {(string.IsNullOrEmpty(item.TechnologyName) ? string.Empty : $" ({item.TechnologyName})")}");
                    row.CreateCell(counter++).SetCellValue(item.ExperienceName);
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(item.Price));
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(item.NoOfResources));
                    row.CreateCell(counter++).SetCellValue(Convert.ToString(item.EstimateHour));
                    row.CreateCell(counter++).SetCellValue(Convert.ToString((item.EstimateHour * item.NoOfResources)));
                    row.CreateCell(counter++).SetCellValue(totalCost.ToString("0.00"));
                    row.CreateCell(counter++).SetCellValue(((totalCost * Convert.ToDecimal(usdCurrency.ExchangeRate))).ToString("0.00"));
                    row.CreateCell(counter++).SetCellValue((totalCost * Convert.ToDecimal(audCurrency.ExchangeRate)).ToString("0.00"));
                    row.CreateCell(counter++).SetCellValue((Math.Truncate(Math.Truncate((totalCost * Convert.ToDecimal(uaeCurrency.ExchangeRate))) / 10)*10).ToString("0.00"));
                    row.CreateCell(counter++).SetCellValue($"{item.NoOfResources} {item.TechnologyName} {(item.NoOfResources > 1 ? $"{item.RoleName}(s)" : item.RoleName)}");

                    counter = 0;
                    row.Height = 350;

                    for (int j = 0; j < headerRow.LastCellNum; j++)
                    {
                        row.GetCell(j).CellStyle = rowRedCellStyleattendeeothercolumn;
                    }
                }
                counter = 0;
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 5000);
                sheet.SetColumnWidth(counter++, 5000);
                sheet.SetColumnWidth(counter++, 5000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 6000);
                sheet.SetColumnWidth(counter++, 4000);
                sheet.SetColumnWidth(counter++, 5000);
                sheet.SetColumnWidth(counter++, 5000);
                sheet.SetColumnWidth(counter++, 5000);
                sheet.SetColumnWidth(counter++, 8000);

                sheet.CreateRow(rowNumber++);
                sheet.CreateRow(rowNumber++);

                counter = 2;
                for (int i = 0; i < source.CalculationResult.Count(); i++)
                {
                    var row = sheet.CreateRow(rowNumber++);
                    var item = source.CalculationResult[i];
                    row.CreateCell(counter++).SetCellValue($"{item.Heading}");
                    row.CreateCell(counter++).SetCellValue($"{item.Pound}");
                    row.CreateCell(counter++).SetCellValue($"{item.USD}");
                    row.CreateCell(counter++).SetCellValue($"{item.AUD}");
                    row.CreateCell(counter++).SetCellValue($"{item.AED}");

                    row.Height = 350;
                    counter = 2;
                    for (int j = counter; j < row.LastCellNum; j++)
                    {
                        row.GetCell(j).CellStyle = rowRedCellStyleattendeeothercolumn;
                    }
                }

                workbook.Write(response);
            }

            response.Position = 0;
            return response;
        }

        #endregion
    }


}
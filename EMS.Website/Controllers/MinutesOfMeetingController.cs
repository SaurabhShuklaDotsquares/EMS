using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.LIBS;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static EMS.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using System.IO;
using EMS.Web.Models.Others;
using Rotativa.AspNetCore;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using EMS.Data.Model;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class MinutesOfMeetingController : BaseController
    {
        #region  Fields and Constructor

        private readonly IMinutesOfMeetingMasterService MinutesOfMeetingMasterService;
        private readonly IMinutesOfMeetingService MinutesOfMeetingService;
        private readonly IMinutesOfMeetingTaskService MinutesOfMeetingTaskService;
        private readonly IMinutesOfMeetingTaskCommentService MinutesOfMeetingTaskCommentService;
        private readonly IUserLoginService userLoginService;
        private readonly IDepartmentService DepartmentService;
        private readonly IPILogService PILogService;
        private readonly IProcessService processService;

        public MinutesOfMeetingController(IMinutesOfMeetingMasterService _MinutesOfMeetingMasterService, IMinutesOfMeetingService _MinutesOfMeetingService, IMinutesOfMeetingTaskService _MinutesOfMeetingTaskService, IMinutesOfMeetingTaskCommentService _MinutesOfMeetingTaskCommentService, IUserLoginService _userLoginService, IDepartmentService _DepartmentService, IPILogService _PILogService, IProcessService _processService)
        {
            MinutesOfMeetingMasterService = _MinutesOfMeetingMasterService;
            MinutesOfMeetingService = _MinutesOfMeetingService;
            MinutesOfMeetingTaskService = _MinutesOfMeetingTaskService;
            MinutesOfMeetingTaskCommentService = _MinutesOfMeetingTaskCommentService;
            userLoginService = _userLoginService;
            DepartmentService = _DepartmentService;
            PILogService = _PILogService ?? throw new ArgumentNullException("_PILogService");
            processService = _processService ?? throw new ArgumentNullException("_processService");
        }

        #endregion

        #region Index
        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate></createddate>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Displaying Minutes of Meeting List by Datatables by Datatables.
        /// </summary>
        /// <returns> returns Minutes of Meeting List.</returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>06-june-2019</createddate>
        [HttpPost]
        public IActionResult Index(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<MeetingMaster>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<MeetingMaster>();

            if (request.Search != null)
            {
                string searchKeyword = request.Search.Value;
                if (!string.IsNullOrEmpty(searchKeyword))
                {
                    filterExpr = filterExpr.And(e => e.Title.Contains(searchKeyword)); //|| e.Description.Contains(searchKeyword));
                }
            }

            var currentUserId = CurrentUser.Uid;

            //filterExpr = filterExpr.And(x => (x.CreatedByUID == currentUserId) || (x.MomMeetings.Where(y => y.AuthorByUID == currentUserId || y.ChairedByUID == currentUserId || (y.MomMeetingParticipant.Where(u => u.Uid == currentUserId || u.Uid == currentUserId).Count() > 0) || (y.MomMeetingDepartment.Where(m => m.DepartmentId == CurrentUser.DeptId).Count() > 0)).Count() > 0));
            filterExpr = filterExpr.And(x => (x.CreatedByUID == currentUserId) ||
                                             (x.MomMeetings.Where(y => y.AuthorByUID == currentUserId ||
                                                                       y.ChairedByUID == currentUserId ||
                                                                       (y.MomMeetingParticipant.Where(u => u.Uid == currentUserId ||
                                                                                                           u.Uid == currentUserId).Count() > 0) ||
                                                                       (y.MomMeetingDepartment.Where(m => m.DepartmentId == CurrentUser.DeptId && (
                                                                                                     m.MomMeeting.UserLogin.PMUid == CurrentUser.PMUid || m.MomMeeting.UserLogin.Uid == CurrentUser.PMUid)).Count() > 0)).Count() > 0));
            int totalCount = 0;
            pagingServices.Filter = filterExpr;
            pagingServices.Sort = (o) =>
            {
                //return o.OrderBy(c => c.ModifiedDate).OrderBy(c => c.CreatedDate);

                return o.OrderByDescending(c => c.MomMeetings.OrderByDescending(m => m.DateOfMeeting).FirstOrDefault() != null ? c.MomMeetings.OrderByDescending(m => m.DateOfMeeting).FirstOrDefault().DateOfMeeting : c.CreatedDate);
            };

            //TempData["MinutesOfMeetingPagingFilter"] = pagingServices;
            var response = MinutesOfMeetingMasterService.GetMinutesOfMeetingByPaging(out totalCount, pagingServices);

            //response = response.OrderByDescending(s => s.MomMeetings.OrderByDescending(m => m.DateOfMeeting).FirstOrDefault() != null ? s.MomMeetings.OrderByDescending(m => m.DateOfMeeting).FirstOrDefault().DateOfMeeting : s.CreatedDate).ToList();

            var st = response.Select((mom, index) => new
            {
                RowIndex = (index + 1) + (request.Start),
                Id = mom.Id,
                MeetingTitle = mom.Title,

                LastMeetingDate = mom.MomMeetings.OrderByDescending(m => m.DateOfMeeting).Select(m => m.DateOfMeeting.ToFormatDateString("MMM d, yyyy")).FirstOrDefault() ?? "No Meetings Added",
                FirstMeetingId = mom.MomMeetings.OrderByDescending(m => m.DateOfMeeting).ThenByDescending(x => x.ModifiedDate).Select(m => m.Id).FirstOrDefault(),
                //MomMeetings = mom.MomMeetings.Select(x =>
                //{
                //    //var participants = x.ParticipantType == (byte)MomMeetingParticipantType.Individual ? x.MomMeetingParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name) : x.MomMeetingDepartment.Select(d => DepartmentService.GetDepartmentById(d.DepartmentId).Name);
                //    //var PmUid = new List<int>();
                //    //if (x.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                //    //{
                //    //    PmUid = x.MomMeetingParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).PMUid.HasValue ? userLoginService.GetUserInfoByID(y.Uid).PMUid.Value : 0).ToList();
                //    //}
                //    //else
                //    //{
                //    //    PmUid = x.MomMeetingDepartment.SelectMany(y => userLoginService.GetUsersDetailsByDeptId(new int[] { y.DepartmentId }).Where(u => u.PMUid != null).Select(u => u.PMUid.Value)).ToList();
                //    //}
                //    //var momMeetings = new
                //    //{
                //    //    x.Id,
                //    //    x.MeetingTitle,
                //    //    MeetingMasterId = x.MeetingMasterID,
                //    //    //Aganda = x.Agenda,
                //    //    Aganda = (x.Agenda.StripHTML().Length > showMaxStringLength ? x.Agenda.StripHTML().TrimLength(showMaxStringLength) : x.Agenda),
                //    //    showAgendaInModle = (x.Agenda.StripHTML().Length > showMaxStringLength ? true : false),
                //    //    AgendaFullText = x.Agenda,
                //    //    venue = x.VenueName,
                //    //    dom = x.DateOfMeeting,
                //    //   ModifiedDate = x.ModifiedDate,
                //    //    DateOfMeeting = $"{x.DateOfMeeting.ToFormatDateString("MMM d, yyyy")}&nbsp;({x.MeetingTime} &nbsp;min)",
                //    //    AuthorBy = "AuthorBy",//userLoginService.GetUserInfoByID(x.AuthorByUID).Name,
                //    //    AuthorByUid = x.AuthorByUID,
                //    //    ChairedByUid = x.ChairedByUID,
                //    //    ChairedBy = "ChairedBy",//userLoginService.GetUserInfoByID(x.ChairedByUID).Name,
                //    //    Participants = string.Join(", ","Shrikant" ),//participants
                //    //    PmUid = "",//PmUid,
                //    //    IsEditable = x.AuthorByUID == currentUserId || x.ChairedByUID == currentUserId,
                //    //    CreatedDate = x.CreatedDate,
                //    //    modalRequired = true,//x.MomMeetingParticipant.Count > 10 ? true : false,
                //    //    ////MomDocuments = x.Momdocuments.Select(s => new MomdocumentDto() { DocumentPath = s.DocumentPath,})
                //    //  //  MomDocuments = x.Momdocuments.Select(s => new MomdocumentDto() { DocumentPath = $"<a href='/Upload/MomDocument/{s.DocumentPath.ToLower()}' class='btn-link i-icon' target='_blank' title='{s.DocumentPath}'><i class='{(s.DocumentPath.ToLower().Contains(".ppt") ? "fa fa-file-powerpoint-o" : s.DocumentPath.ToLower().Contains(".doc") ? "fa fa-file-word-o" : s.DocumentPath.ToLower().Contains(".xls") ? "fa fa-file-excel-o" : s.DocumentPath.ToLower().Contains(".txt") ? "fa fa-file-text-o" : s.DocumentPath.ToLower().Contains(".rar") || s.DocumentPath.ToLower().Contains(".zip") ? "fa fa-file-zip-o" : "fa fa-file-image-o")}'></i>{s.DocumentPath.ToLower()}</a>" })
                //    //};
                //    return momMeetings;
                //}).OrderByDescending(x => x.dom).ThenByDescending(x => x.ModifiedDate),//.Where(x => x.Participants.Contains(CurrentUser.Name) || x.ChairedByUid == currentUserId || x.AuthorByUid == currentUserId || x.PmUid.Contains(currentUserId) || x.Participants.Contains(CurrentUser.DeptId.ToString())).OrderBy(x => x.DateOfMeeting).OrderBy(x => x.Id),
                //}).OrderBy(x => x.dom),//.Where(x => x.Participants.Contains(CurrentUser.Name) || x.ChairedByUid == currentUserId || x.AuthorByUid == currentUserId || x.PmUid.Contains(currentUserId) || x.Participants.Contains(CurrentUser.DeptId.ToString())).OrderBy(x => x.DateOfMeeting).OrderBy(x => x.Id),
                EditAllowed = mom.CreatedByUID == currentUserId
            });
            var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
            return Json(new { draw = draw, recordsFiltered = totalCount, recordsTotal = totalCount, data = st });

        }



        /// <summary>
        /// Displaying Minutes of Meeting child by Datatables by Datatables.
        /// </summary>
        /// <returns> returns Minutes of Meeting List.</returns>
        /// <createdby>Shrikant Tiwari</createdby>
        /// <createddate>01-Aug-2022</createddate>
        public IActionResult GetMOMByID(int ID = 66)
        {
           // var response = MinutesOfMeetingMasterService.GetMinutesOfMeetingFindById(ID);
            int showMaxStringLength = 500;
            IList<Object> list = new List<Object>();
            var currentUserId = CurrentUser.Uid;

            // Settings.  
            var lst = MinutesOfMeetingService.GETMOMListByMeetingMasterIDSP(ID).ToList();







            List<MomdocumentDto> MomDoclst = new List<MomdocumentDto>();

            var MomMeetings = lst.Select(Q =>
            {
                //var participants = Q.ParticipantType == (byte)MomMeetingParticipantType.Individual ? Q.MomMeetingParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name) : Q.MomMeetingDepartment.Select(d => DepartmentService.GetDepartmentById(d.DepartmentId).Name);
              //  var PmUid = new List<int>();
               // if (Q.ParticipantType == (byte)MomMeetingParticipantType.Individual)
             //   {
                   // PmUid = Q.MomMeetingParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).PMUid.HasValue ? userLoginService.GetUserInfoByID(y.Uid).PMUid.Value : 0).ToList();
            //    }
             //   else
              //  {
            //        PmUid = Q.MomMeetingDepartment.SelectMany(y => userLoginService.GetUsersDetailsByDeptId(new int[] { y.DepartmentId }).Where(u => u.PMUid != null).Select(u => u.PMUid.Value)).ToList();
             //   }

                var momMeetings = new
                {

                    Q.Id,
                    Q.MeetingTitle,
                    MeetingMasterId = Q.MeetingMasterID,
                    //Aganda = Q.Agenda,
                    Aganda = (Q.Agenda.StripHTML().Length > showMaxStringLength ? Q.Agenda.StripHTML().TrimLength(showMaxStringLength) : Q.Agenda),
                    showAgendaInModle = (Q.Agenda.StripHTML().Length > 500 ? true : false),
                    AgendaFullText = Q.Agenda,
                    venue = Q.VenueName,
                    dom = Q.DateOfMeeting,
                    ModifiedDate = Q.ModifiedDate,
                    DateOfMeeting = $"{Q.DateOfMeeting.ToFormatDateString("MMM d, yyyy")}&nbsp;({Q.MeetingTime} &nbsp;min)",
                    AuthorBy =Q.AuthorBy,//userLoginService.GetUserInfoByID(Q.AuthorByUID).Name,
                    AuthorByUid = Q.AuthorByUid,
                    ChairedByUid = Q.ChairedByUID,
                    ChairedBy =Q.ChairedBy,// userLoginService.GetUserInfoByID(Q.ChairedByUID).Name,
                    Participants = Q.participants != null ? Q.participants:"N/A",//string.Join(", ", ""),//participants
                    PmUid = "",
                    IsEditable =  Q.AuthorByUid == currentUserId || Q.ChairedByUID == currentUserId,
                    CreatedDate = Q.ModifiedDate,
                    modalRequired = Q.participants!=null?(Q.participants.Split(',').Length > 10 ? true : false):false,//Q.MomMeetingParticipant.Count > 10 ? true : false,
                    MomDocuments = Q.MomDocuments!=null? Q.MomDocuments.Split(',').Select(x=>new MomdocumentDto() { DocumentPath = $"<a href='/Upload/MomDocument/{x.ToLower()}' class='btn-link i-icon' target='_blank' title='{x}'><i class='{(x.ToLower().Contains(".ppt") ? "fa fa-file-powerpoint-o" : x.ToLower().Contains(".doc") ? "fa fa-file-word-o" : x.ToLower().Contains(".xls") ? "fa fa-file-excel-o" :x.ToLower().Contains(".txt") ? "fa fa-file-text-o" : x.Contains(".rar") || x.Contains(".zip") ? "fa fa-file-zip-o" : "fa fa-file-image-o")}'></i>{x.ToLower()}</a>" }) : MomDoclst//Q.Momdocuments.Select(s => new MomdocumentDto() { DocumentPath = $"<a href='/Upload/MomDocument/{s.DocumentPath.ToLower()}' class='btn-link i-icon' target='_blank' title='{s.DocumentPath}'><i class='{(s.DocumentPath.ToLower().Contains(".ppt") ? "fa fa-file-powerpoint-o" : s.DocumentPath.ToLower().Contains(".doc") ? "fa fa-file-word-o" : s.DocumentPath.ToLower().Contains(".Qls") ? "fa fa-file-eQcel-o" : s.DocumentPath.ToLower().Contains(".tQt") ? "fa fa-file-teQt-o" : s.DocumentPath.ToLower().Contains(".rar") || s.DocumentPath.ToLower().Contains(".zip") ? "fa fa-file-zip-o" : "fa fa-file-image-o")}'></i>{s.DocumentPath.ToLower()}</a>" })


                };
                return momMeetings;
            }).OrderByDescending(x => x.dom).ThenByDescending(x => x.ModifiedDate);

            //foreach (var x in lst)
            //{

            //    list.Add(


            //         new
            //         {
            //             x.Id,
            //             x.MeetingTitle,
            //             MeetingMasterId = x.MeetingMasterID,
            //             //Aganda = x.Agenda,
            //             Aganda = (x.Agenda.StripHTML().Length > showMaxStringLength ? x.Agenda.StripHTML().TrimLength(showMaxStringLength) : x.Agenda),
            //             showAgendaInModle = (x.Agenda.StripHTML().Length > showMaxStringLength ? true : false),
            //             AgendaFullText = x.Agenda,
            //             venue = x.VenueName,
            //             dom = x.DateOfMeeting,
            //             ModifiedDate = x.ModifiedDate,
            //             DateOfMeeting = $"{x.DateOfMeeting.ToFormatDateString("MMM d, yyyy")}&nbsp;({x.MeetingTime} &nbsp;min)",
            //             AuthorBy = userLoginService.GetUserInfoByID(x.AuthorByUid).Name,
            //             AuthorByUid = x.AuthorByUid,
            //             ChairedByUid = x.ChairedByUID,
            //             ChairedBy = x.ChairedBy,
            //             Participants = x.participants,//participants
            //             PmUid = "",
            //             IsEditable = x.AuthorByUid == currentUserId || x.ChairedByUID == currentUserId,
            //             CreatedDate = x.DateOfMeeting,
            //             modalRequired = x.participants.Split(',').Length > 10 ? true : false,
            //             ////MomDocuments = x.Momdocuments.Select(s => new MomdocumentDto() { DocumentPath = s.DocumentPath,})
            //             MomDocuments = ""// x.MomDocuments.Select(s => new MomdocumentDto() { DocumentPath = $"<a href='/Upload/MomDocument/{x.DocumentPath.ToLower()}' class='btn-link i-icon' target='_blank' title='{s.DocumentPath}'><i class='{(s.DocumentPath.ToLower().Contains(".ppt") ? "fa fa-file-powerpoint-o" : s.DocumentPath.ToLower().Contains(".doc") ? "fa fa-file-word-o" : s.DocumentPath.ToLower().Contains(".xls") ? "fa fa-file-excel-o" : s.DocumentPath.ToLower().Contains(".txt") ? "fa fa-file-text-o" : s.DocumentPath.ToLower().Contains(".rar") || s.DocumentPath.ToLower().Contains(".zip") ? "fa fa-file-zip-o" : "fa fa-file-image-o")}'></i>{s.DocumentPath.ToLower()}</a>" })


            //         }
            //        );
            //}


            return Json(MomMeetings);


        }

        #endregion
        #region Add/Edit Meeting Master
        /// <summary>
        ///  Get Minutes Of Meeting Master By Id 
        /// </summary>
        /// <returns> returns addedit partial view with models.</returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>07-06-2019</createddate>
        [HttpGet]
        public ActionResult AddEdit(int? id)
        {
            try
            {
                MeetingMasterDto model = new MeetingMasterDto();

                if (id.HasValue && id.Value > 0)
                {
                    var entity = MinutesOfMeetingMasterService.GetMinutesOfMeetingFindById(id.Value);
                    if (entity != null)
                    {
                        if (entity.CreatedByUID == CurrentUser.Uid)
                        {
                            model.Id = entity.Id;
                            model.Title = entity.Title;
                        }
                        else
                        {
                            return MessagePartialView("Unauthorized access");
                        }
                    }
                }
                return PartialView("_AddEdit", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Add/Edit Meetings with parameters
        /// </summary>
        /// <returns> return json message either added or updated. </returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>08-06-2019</createddate>
        [HttpPost]
        public ActionResult AddEdit(MeetingMasterDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CreateByUid = CurrentUser.Uid;
                    if (model.Id > 0)
                    {
                        model.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        model.CreatedDate = DateTime.Now;
                        model.ModifiedDate = DateTime.Now;
                    }
                    var result = MinutesOfMeetingMasterService.Save(model);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Minutes of Meeting has been {0} successfully", model.Id > 0 ? "updated" : "added")
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
            return CreateModelStateErrors();
        }

        #endregion

        #region Add Edit Mom Meetings
        /// <summary>
        /// To add meetings
        /// </summary>
        /// <returns> it returns partial view with respective model</returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>08-06-2019</createddate>
        [HttpGet]
        public ActionResult AddEditMeeting(int masterId, int? id)
        {
            MomMeetingDto model = new MomMeetingDto();
            try
            {
                if (id.HasValue && id.Value > 0)
                {
                    var entity = MinutesOfMeetingService.GetMinutesOfMeetingFindById(id.Value);
                    if (entity != null)
                    {
                        if (entity.AuthorByUID == CurrentUser.Uid || entity.ChairedByUID == CurrentUser.Uid)
                        {
                            var users = userLoginService.GetUsers(true);
                            var Department = DepartmentService.GetActiveDepartments();
                            //model.AuthorByList = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString(), Selected = entity.UserLogins.Any(s => s.Uid == n.Uid) }).ToList();
                            model.AuthorByList = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString(), Selected = entity.MomMeetingParticipant.Any(s => s.Uid == CurrentUser.Uid) }).ToList();
                            model.ChairedByList = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString(), Selected = entity.MomMeetingParticipant.Any(s => s.Uid == n.Uid) }).ToList();
                            //model.PaticipantList = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString(), Selected = entity.MomMeetingParticipant.Any(s => s.Uid == n.Uid) }).ToList();
                            //model.PaticipantList = users.Where(a => a.PMUid != null).Select(n => new SelectListItem { Text = n.Name + ' ' + GetPMName(n.PMUid.Value), Value = n.Uid.ToString(), Selected = entity.MomMeetingParticipant.Any(s => s.Uid == n.Uid) }).ToList();

                            //model.PaticipantList = users.Where(a => a.PMUid != null).Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu!=null? '(' + n.Pmu.Name + ')':""), Value = n.Uid.ToString(), Selected = entity.MomMeetingParticipant.Any(s => s.Uid == n.Uid) }).ToList();
                            model.PaticipantList = users.Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString(), Selected = entity.MomMeetingParticipant.Any(s => s.Uid == n.Uid) }).ToList();
                            model.GroupList = Department.Select(n => new SelectListItem { Text = n.Name, Value = n.DeptId.ToString(), Selected = entity.MomMeetingDepartment.Any(d => d.DepartmentId == n.DeptId) }).ToList();

                            model.Id = entity.Id;
                            model.MeetingMasterID = entity.MeetingMasterID;
                            model.MeetingMasterTitle = MinutesOfMeetingMasterService.GetMinutesOfMeetingFindById(masterId).Title;
                            model.MeetingTitle = entity.MeetingTitle;
                            model.VenueName = entity.VenueName;
                            model.MeetingTime = entity.MeetingTime;
                            model.MeetingStartTime = entity.MeetingStartTime.ToString();
                            model.DateOfMeetings = entity.DateOfMeeting.ToFormatDateString("dd/MM/yyyy");
                            model.Agenda = entity.Agenda;
                            model.Notes = entity.Notes;
                            model.AuthorByUID = entity.AuthorByUID;
                            model.ChairedByUID = entity.ChairedByUID;
                            model.ParticipantType = (MomMeetingParticipantType)entity.ParticipantType;
                            model.MomDocuments = entity.Momdocuments.Select(s => new MomdocumentDto() { Id = s.Id, DocumentPath = s.DocumentPath }).ToList();
                        }
                        else
                        {
                            return MessagePartialView("Unauthorized access");
                        }
                    }
                    else
                    {
                        entity.MeetingMasterID = id ?? 0;
                    }
                }
                else
                {


                    var prevmeeting = MinutesOfMeetingService.GetPreviousMinutesofMeetingByMeetingMasterId(masterId);

                    var users = userLoginService.GetUsers(true);
                    var department = DepartmentService.GetActiveDepartments();

                    //  var preParticipants = MinutesOfMeetingService.GetMinutesofMeetingPreviousMeetingUsersByMeetingMasterId(masterId, CurrentUser.Uid);
                    //  var preDepartmentss = MinutesOfMeetingService.GetMinutesofMeetingPreviousMeetingDepartmentsByMeetingMasterId(masterId, CurrentUser.Uid);
                    if (prevmeeting != null)
                    {
                        var preParticipants = MinutesOfMeetingService.GetMinutesofMeetingPreviousMeetingUsersByMeetingMasterId(masterId, prevmeeting.AuthorByUID);
                        var preDepartmentss = MinutesOfMeetingService.GetMinutesofMeetingPreviousMeetingDepartmentsByMeetingMasterId(masterId, prevmeeting.AuthorByUID);
                        if (preParticipants == null || preParticipants.Count == 0)
                        {
                            if (preDepartmentss == null || preDepartmentss.Count == 0)
                            {
                                model.ParticipantType = MomMeetingParticipantType.Individual;
                            }
                            else
                            {
                                model.ParticipantType = MomMeetingParticipantType.Group;
                            }
                        }
                        else
                        {
                            model.ParticipantType = MomMeetingParticipantType.Individual;
                        }

                        // model.MeetingTitle = prevmeeting.MeetingTitle;
                        model.VenueName = prevmeeting.VenueName;
                        model.DateOfMeetings = prevmeeting.DateOfMeeting.ToFormatDateString("dd/MM/yyyy");
                        model.MeetingTime = 60;
                        // model.MeetingStartTime = prevmeeting.MeetingStartTime.ToString();
                        // model.ChairedByUID = prevmeeting.ChairedByUID;
                        model.Agenda = prevmeeting.Agenda;
                        // model.Notes = prevmeeting.Notes;
                        model.PaticipantList = users.Where(a => a.PMUid != null).Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString(), Selected = preParticipants != null && preParticipants.Any(s => s.Uid == n.Uid) }).ToList();
                        model.GroupList = department.Select(n => new SelectListItem { Text = n.Name, Value = n.DeptId.ToString(), Selected = preDepartmentss != null && preDepartmentss.Any(s => s.DepartmentId == n.DeptId) }).ToList();
                    }
                    else
                    {
                        model.ParticipantType = MomMeetingParticipantType.Individual;
                        model.DateOfMeetings = DateTime.Now.ToFormatDateString("dd/MM/yyyy");
                        model.MeetingTime = 60;
                        //model.PaticipantList = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
                        //model.PaticipantList = users.Where(a => a.PMUid != null).Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString() }).ToList();
                        model.PaticipantList = users.Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString() }).ToList();
                        model.GroupList = department.Select(n => new SelectListItem { Text = n.Name, Value = n.DeptId.ToString() }).ToList();
                    }

                    // model.ParticipantType = Enums.MomMeetingParticipantType.Individual;

                    model.MeetingMasterID = masterId;
                    model.MeetingMasterTitle = MinutesOfMeetingMasterService.GetMinutesOfMeetingFindById(masterId).Title;
                    model.AuthorByUID = CurrentUser.Uid;
                    model.AuthorByList = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString(), Selected = model.AuthorByUID == n.Uid }).ToList();
                    model.ChairedByList = users.Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString(), Selected = model.ChairedByUID == n.Uid }).ToList();

                }

                return PartialView("_AddEditMeeting", model);

                //return View(model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }

        }


        /// <summary>
        /// Show meetings agenda.
        /// </summary>
        /// <returns> it returns partial view with respective model</returns>
        /// <createdby>RDK</createdby>
        /// <createddate>30-09-2019</createddate>
        [HttpGet]
        public ActionResult ShowMeetingAgenda(int masterId, int? id)
        {
            MomMeetingDto model = new MomMeetingDto();
            try
            {
                if (id.HasValue && id.Value > 0)
                {
                    var entity = MinutesOfMeetingService.GetMinutesOfMeetingFindById(id.Value);
                    if (entity != null)
                    {
                        model.Agenda = entity.Agenda;
                    }
                }
                return PartialView("_ShowAgendaInModel", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet]
        public ActionResult ShowMeetingParticipant(int masterId, int? id)
        {
            MomMeetingDto model = new MomMeetingDto();
            try
            {
                if (id.HasValue && id.Value > 0)
                {
                    var entity = MinutesOfMeetingService.GetMinutesOfMeetingFindById(id.Value);
                    if (entity != null)
                    {
                        var participants = entity.MomMeetingParticipant.ToList();
                        foreach (var item in participants)
                        {
                            var participantName = userLoginService.GetUserInfoByID(item.Uid).Name;
                            model.selectedParticpants += participantName + ", ";
                        }
                    }
                }
                return PartialView("_ShowAllParticipentsModel", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }


        /// <summary>
        /// To add meetings 
        /// </summary>
        /// <param name="model">MomMeeting Details  </param>
        /// <returns>add meetings  and return message for success or failure.</returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        [HttpPost]
        public ActionResult AddEditMeeting(MomMeetingDto model, IFormFileCollection docs)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    string requiredExtension = SiteKey.Extension + "," + SiteKey.ImageExtension;
                    foreach (var item in docs)
                    {
                        var fileExt = Path.GetExtension(item.FileName.ToLower());
                        if (!requiredExtension.Contains(fileExt))
                        {
                            return MessagePartialView($"The file(s) which you are trying to upload does not support, please try with following ({requiredExtension}).");
                        }
                    }


                    model.AuthorByUID = CurrentUser.Uid;

                    List<string> MomDocuments = new List<string>();

                    foreach (var item in docs)
                    {
                        string FileName = GeneralMethods.SaveFile(item, "Upload/MomDocument/", "");
                        model.MomDocuments.Add(new MomdocumentDto() { DocumentPath = FileName });
                    }

                    var result = MinutesOfMeetingService.Save(model);
                    if (result != null && result.Id > 0)
                    {
                        if (model.SendEmail)
                        {
                            foreach (var item in result.Momdocuments)
                            {
                                string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", item.DocumentPath);
                                MomDocuments.Add(FilePath);
                            }

                            foreach (var itemMomTask in result.MomMeetingTasks)
                            {
                                foreach (var itemTaskDoc in itemMomTask.MomMeetingTaskDocuments)
                                {
                                    string filePathForActionDoc = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", itemTaskDoc.DocumentPath);
                                    if (System.IO.File.Exists(filePathForActionDoc))
                                    {
                                        MomDocuments.Add(filePathForActionDoc);
                                    }
                                }
                            }

                            SendMinutesOfMeetingEmailToParticipants(result, MomDocuments);
                        }

                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Minutes of Meeting has been {0} successfully", model.Id > 0 ? "updated" : "added")
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
            return CreateModelStateErrors();
        }


        /// <summary>
        /// To Edit meetings 
        /// </summary>
        /// <param name="model">MomMeeting Details  </param>
        /// <returns>Edit meetings  and return message for success or failure.</returns>
        ///  <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        [HttpPost]
        public ActionResult EditMeeting(MomMeetingDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var result = MinutesOfMeetingService.Save(model);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Minutes of Meeting has been {0} successfully", model.Id > 0 ? "updated" : "added")
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
                }
            }
            return CreateModelStateErrors();

        }
        #endregion

        #region [View Meeting]
        [HttpGet]
        public ActionResult ViewMeeting(int masterId, int? id)
        {
            MomMeetingDto model = new MomMeetingDto();
            try
            {
                if (id.HasValue && id.Value > 0)
                {
                    var entity = MinutesOfMeetingService.GetMinutesOfMeetingFindById(id.Value);
                    if (entity != null)
                    {
                        if (entity.AuthorByUID == CurrentUser.Uid || entity.ChairedByUID == CurrentUser.Uid || entity.MomMeetingParticipant.Any(m => m.Uid == CurrentUser.Uid))
                        {
                            var users = userLoginService.GetAllUsersList();
                            model.AuthorName = users.FirstOrDefault(x => x.Uid == entity.AuthorByUID).Name;
                            model.ChairedName = users.FirstOrDefault(x => x.Uid == entity.ChairedByUID).Name;

                            if (entity.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                            {
                                model.StrPaticipant = string.Join(", ", users.Where(a => a.PMUid != null && entity.MomMeetingParticipant.Any(s => s.Uid == a.Uid)).Select(n => (n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""))));
                            }
                            else
                            {
                                var Department = DepartmentService.GetActiveDepartments();
                                model.StrPaticipant = string.Join(", ", Department.Where(x => entity.MomMeetingDepartment.Any(d => d.DepartmentId == x.DeptId)).Select(n => n.Name));
                            }

                            model.Id = entity.Id;
                            model.MeetingMasterID = entity.MeetingMasterID;
                            model.MeetingMasterTitle = MinutesOfMeetingMasterService.GetMinutesOfMeetingFindById(masterId).Title;
                            model.MeetingTitle = entity.MeetingTitle;
                            model.VenueName = entity.VenueName;
                            model.MeetingTime = entity.MeetingTime;
                            model.MeetingStartTime = entity.MeetingStartTime.ToString();
                            model.DateOfMeetings = entity.DateOfMeeting.ToFormatDateString("dd/MM/yyyy");
                            model.Agenda = entity.Agenda;
                            model.Notes = entity.Notes;
                            model.AuthorByUID = entity.AuthorByUID;
                            model.ChairedByUID = entity.ChairedByUID;
                            model.ParticipantType = (MomMeetingParticipantType)entity.ParticipantType;
                            model.MomDocuments = entity.Momdocuments.Select(s => new MomdocumentDto() { Id = s.Id, DocumentPath = s.DocumentPath }).ToList();
                            model.MomMeetingTasks = entity.MomMeetingTasks.Select(x => new MomMeetingTaskDto
                            {
                                Task = x.Task,
                                Status = (MomMeetingStatus)x.Status,
                                Remark = x.Remark,
                                Priority = (Priority)x.Priority,
                                PaticipantsList = string.Join(", ", x.MomMeetingTaskParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name).ToList()),
                            }).ToList();
                        }
                        else
                        {
                            return MessagePartialView("Unauthorized access");
                        }
                    }
                    else
                    {
                        entity.MeetingMasterID = id ?? 0;
                    }
                }
                return PartialView("_ViewMeeting", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }
        #endregion

        #region [Delete Meeting]
        [HttpGet]
        public ActionResult DeleteMeeting(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete?",
                Size = Enums.ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Record ?" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }
            });
        }

        [HttpPost]
        public ActionResult deleteMeeting(int masterId, int id)
        {
            try
            {
                if (id > 0)
                {
                    var result = MinutesOfMeetingService.Delete(id);
                    if (result)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Record deleted successfully.", IsSuccess = true });
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete task" });
                    }
                }

                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = "Unable to delete task" });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<string> { ErrorMessage = ex.Message });
            }
        }
        #endregion

        #region Mom Meetings Task
        /// <summary>
        /// Get Minutes Of Meeting By Id and View Tasks
        /// </summary>
        /// <returns>return model</returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        [HttpGet]
        public ActionResult ViewTask(int id)
        {
            MomMeetingDto model = new MomMeetingDto();
            var entity = MinutesOfMeetingService.GetMinutesOfMeetingFindById(id);
            model.Id = entity.Id;
            model.MeetingTitle = entity.MeetingTitle;
            model.MeetingMasterID = entity.MeetingMasterID;
            model.Status = MomMeetingStatus.Pending;
            model.DateOfMeetings = entity.DateOfMeeting.ToFormatDateString("MMM d, yyyy");


            var participants = new List<SelectListItem>();

            List<MomMeetingTask> responseParticipants = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskByMeetingMasterId(entity.MeetingMasterID, entity.CreatedDate).OrderBy(c => c.CreatedDate).OrderByDescending(c => c.ModifiedDate).ToList();

            participants = responseParticipants.SelectMany(n => n.MomMeetingTaskParticipant.Select(m => new SelectListItem { Text = m.U.Name, Value = m.U.Uid.ToString() })).ToList();

            participants = participants.GroupBy(x => x.Value).Select(x => x.First()).OrderBy(m => m.Text).ToList();
            model.PaticipantList = participants;

            return View(model);
        }

        /// <summary>
        /// To View task  by datatables by meeting id, topic id , taskstatus id
        /// </summary>
        /// <returns> return datatables for task </returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        [HttpPost]
        public IActionResult ViewTask(IDataTablesRequest request, int meetingId, int topicId, string statusId, string filterParticipants)
        {
            statusId = statusId ?? "0";
            int futureActionStatusid = 5;
            int completedActionStatusid = 4;
            int[] int_taskstatusId = { };
            string[] taskstatusId = null;

            if (statusId.HasValue())
            {
                if (statusId.Contains("0"))
                {
                    taskstatusId = Enum.GetValues(typeof(MomMeetingStatus)).Cast<int>().Where(x => x != (byte)MomMeetingStatus.Completed).Select(x => x.ToString()).ToArray();
                    Array.Resize(ref taskstatusId, taskstatusId.Length + 2);
                    taskstatusId[taskstatusId.Length - 2] = "5";
                    taskstatusId[taskstatusId.Length - 1] = "0";
                }
                else
                {
                    taskstatusId = statusId.Split(',').ToArray();
                }

            }

            if (taskstatusId != null && !taskstatusId.Contains(""))
            {
                int_taskstatusId = Array.ConvertAll(taskstatusId, int.Parse);
            }

            var meetingDetails = MinutesOfMeetingService.GetMinutesOfMeetingFindById(meetingId);
            var pagingServices = new PagingService<MomMeetingTask>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<MomMeetingTask>();
            int totalCount = 0;

            #region Logic
            if (int_taskstatusId.Contains(0))
            {
                if (filterParticipants != null && filterParticipants != string.Empty)
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && x.Status != (byte)MomMeetingStatus.Completed && x.MomMeetingTaskParticipant.Any(n => n.Uid.ToString() == filterParticipants.ToString()));
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && x.Status != (byte)MomMeetingStatus.Completed);
                }
            }
            else if (!int_taskstatusId.Contains(0) && (int_taskstatusId.Contains((byte)MomMeetingStatus.Pending) || int_taskstatusId.Contains((byte)MomMeetingStatus.Ongoing) || int_taskstatusId.Contains((byte)MomMeetingStatus.Delayed)))
            {
                if (filterParticipants != null && filterParticipants != string.Empty)
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && int_taskstatusId.Contains(x.Status) && x.TargetDate <= DateTime.Now && x.MomMeetingTaskParticipant.Any(n => n.Uid.ToString() == filterParticipants.ToString()));
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && int_taskstatusId.Contains(x.Status) && x.TargetDate <= DateTime.Now);
                }
            }
            else if (!int_taskstatusId.Contains(0) && int_taskstatusId.Contains(completedActionStatusid))
            {
                if (filterParticipants != null && filterParticipants != string.Empty)
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && x.Status == (byte)MomMeetingStatus.Completed && x.MomMeetingTaskParticipant.Any(n => n.Uid.ToString() == filterParticipants.ToString()));
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && x.Status == (byte)MomMeetingStatus.Completed);
                }
            }

            else if (!int_taskstatusId.Contains(0) && int_taskstatusId.Contains(futureActionStatusid))
            {
                if (filterParticipants != null && filterParticipants != string.Empty)
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && x.TargetDate >= DateTime.Now && x.Status != (byte)MomMeetingStatus.Completed && x.MomMeetingTaskParticipant.Any(n => n.Uid.ToString() == filterParticipants.ToString()));
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && x.TargetDate >= DateTime.Now && x.Status != (byte)MomMeetingStatus.Completed);
                }
            }
            else
            {
                if (filterParticipants != null && filterParticipants != string.Empty)
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && int_taskstatusId.Contains(x.Status) && x.MomMeetingTaskParticipant.Any(n => n.Uid.ToString() == filterParticipants.ToString()));
                }
                else
                {
                    filterExpr = filterExpr.And(x => x.MomMeeting.MeetingMasterID == topicId && x.MomMeeting.CreatedDate <= meetingDetails.CreatedDate && int_taskstatusId.Contains(x.Status));
                }
            }
            #endregion

            pagingServices.Filter = filterExpr;
            pagingServices.Sort = (o) =>
            {
                return o.OrderBy(c => c.CreatedDate).OrderByDescending(c => c.ModifiedDate);
            };


            var response = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskByPaging(out totalCount, pagingServices).OrderBy(c => c.CreatedDate).OrderByDescending(c => c.ModifiedDate);

            return DataTablesJsonResult(totalCount, request, response.Select((taskList, index) => new
            {
                RowIndex = (index + 1) + (request.Start),
                Id = taskList.Id,
                MeetingId = meetingId,
                Task = "<strong>" + taskList.Task + "</strong><br/><i>(Meeting Date " + taskList.MomMeeting.DateOfMeeting.ToFormatDateString("MMM d, yyyy") + " in " + MinutesOfMeetingService.GetMinutesOfMeetingFindById(taskList.MomMeetingId).MeetingTitle + ")</i>",
                TaskStatus = taskList.Status == futureActionStatusid ? "Future Action" : Convert.ToString((MomMeetingStatus)taskList.Status),
                Remark = taskList.Remark,
                Priority = taskList.Priority != null ? Extensions.GetDescription((Enums.Priority)taskList.Priority) : string.Empty,
                TargetDate = taskList.TargetDate.ToFormatDateString("MMM d, yyyy"),
                PaticipantsList = string.Join(", ", taskList.MomMeetingTaskParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name).ToList()),
                IsEditable = (taskList.MomMeetingId == meetingId) && (taskList.MomMeeting.AuthorByUID == CurrentUser.Uid || taskList.MomMeeting.ChairedByUID == CurrentUser.Uid) && (taskList.Status != (byte)MomMeetingStatus.Completed),
                IsNotCompleted = taskList.Status != (byte)MomMeetingStatus.Completed,
                IsDiscussed = taskList.MomMeetingTaskTimeLine.Count > 0 ? taskList.MomMeetingTaskTimeLine.OrderByDescending(m => m.CreatedDate).FirstOrDefault().CreatedDate >= meetingDetails.DateOfMeeting && taskList.MomMeetingTaskTimeLine.OrderByDescending(m => m.CreatedDate).FirstOrDefault().MomMeetingId == meetingDetails.Id : false
            }));
        }

        /// <summary>
        /// To add/edit task
        /// </summary>
        /// <returns> return patial view with model </returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        [HttpGet]
        public ActionResult AddEditTask(int meetingId, int? id)
        {
            MomMeetingTaskDto model = new MomMeetingTaskDto();
            try
            {
                MomMeetingTask entity = null;
                if (id.HasValue && id.Value > 0)
                {
                    entity = id.Value > 0 ? MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskFindById(id.Value) : new MomMeetingTask();
                    if (entity != null && entity.MomMeeting != null)
                    {
                        var isEditable = (entity.MomMeeting.AuthorByUID == CurrentUser.Uid ||
                                 entity.MomMeeting.ChairedByUID == CurrentUser.Uid) && (entity.Status != (byte)MomMeetingStatus.Completed);
                        if (isEditable)
                        {
                            // ok
                        }
                        else
                        {
                            return MessagePartialView("You don't have permission to edit MOM Action");
                        }
                    }
                }

                model.MomMeetingId = meetingId;
                var momMeeting = MinutesOfMeetingService.GetMinutesOfMeetingFindById(meetingId);

                var participants = new List<SelectListItem>();

                //var authorParticipants = userLoginService.GetUserInfoByID(momMeeting.AuthorByUID);
                //var chairedParticipants = userLoginService.GetUserInfoByID(momMeeting.ChairedByUID);

                SelectListGroup participantGroup = new SelectListGroup() { Name = "A. Meeting Participants" };
                SelectListGroup otherGroup = new SelectListGroup() { Name = "B. Outside Participants" };
                if (momMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                {
                    //participants = momMeeting.MomMeetingParticipant.Select(n => new SelectListItem { Text = n.U.Name, Value = n.U.Uid.ToString() }).ToList();
                    //participants = momMeeting.MomMeetingParticipant.Where(a => a.U.PMUid != null).Select(n => new SelectListItem { Text = n.U.Name + ' ' + GetPMName(n.U.PMUid.Value), Value = n.U.Uid.ToString() }).ToList();
                    //var momMeetingparticipants = momMeeting.MomMeetingParticipant.Where(a => a.U.PMUid != null).Select(n => new SelectListItem { Text = n.U.Name + ' ' + (n.U.Pmu != null ? '(' + n.U.Pmu.Name + ')' : ""), Value = n.U.Uid.ToString(), Group = participantGroup }).ToList();
                    var momMeetingparticipants = momMeeting.MomMeetingParticipant.Select(n => new SelectListItem { Text = n.U.Name + ' ' + (n.U.Pmu != null ? '(' + n.U.Pmu.Name + ')' : ""), Value = n.U.Uid.ToString(), Group = participantGroup }).ToList();
                    //var otherParticipant= new List<SelectListItem>();
                    var users = userLoginService.GetUsers(true);
                    //List<SelectListItem> employees = users.Where(a => a.PMUid != null).Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString(), Group = otherGroup }).ToList();
                    List<SelectListItem> employees = users.Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString(), Group = otherGroup }).ToList();
                    employees.RemoveAll(x => momMeetingparticipants.Any(y => y.Value == x.Value));// removing meeting participant
                    participants.AddRange(momMeetingparticipants);
                    participants.AddRange(employees);
                }
                else
                {
                    if (IsSpecialUser())
                    {
                        participants = userLoginService.GetUsersDetailsByDeptId(momMeeting.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray()).Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
                    }
                    else
                    {
                        participants = userLoginService.GetUsersDetailsByDeptId(momMeeting.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray())
                            .Where(n => (n.PMUid == CurrentUser.Uid || n.Uid == CurrentUser.Uid || n.Uid == CurrentUser.PMUid || n.PMUid == CurrentUser.PMUid))
                            .Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
                    }

                }

                if (id.HasValue && id.Value > 0)
                {
                    //var entity = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskFindById(id.Value);
                    if (entity != null)
                    {
                        model.Id = entity.Id;
                        model.Task = entity.Task;
                        model.MeetingTitle = momMeeting.MeetingTitle;
                        model.Status = (MomMeetingStatus)entity.Status;
                        model.Priority = entity.Priority != null ? (Enums.Priority)entity.Priority : Priority.Medium;
                        model.TargetDates = entity.TargetDate.ToFormatDateString("dd/MM/yyyy");
                        model.Remark = entity.Remark;
                        model.MomMeetingTaskDocuments = entity.MomMeetingTaskDocuments.Select(s => new MomMeetingTaskDocumentDto() { Id = s.Id, DocumentPath = s.DocumentPath }).ToList();
                        if (entity.MomMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                        {
                            //participants = entity.MomMeetingTaskParticipant.Select(n => new SelectListItem { Text = n.U.Name, Value = n.U.Uid.ToString(), Selected = entity.MomMeetingTaskParticipant.Any(s => s.U.Uid == n.U.Uid) }).ToList();

                            //participants = entity.MomMeeting.MomMeetingParticipant.Select(n => new SelectListItem { Text = n.U.Name, Value = n.U.Uid.ToString(), Selected = entity.MomMeetingTaskParticipant.Any(s => s.U.Uid == n.U.Uid) }).ToList();

                            //AddOtherParticipants(ref participants, momMeeting.MomMeetingParticipant, entity.MomMeetingTaskParticipant);

                            //participants = participants.Select(n => new SelectListItem { Text = n.Text, Value = n.Value.ToString(), Selected = entity.MomMeetingTaskParticipant.Any(s => s.U.Uid.ToString() == n.Value),Group=n.Group }).OrderBy(o => o.Text).ToList();
                            participants = participants.Select(n => new SelectListItem
                            { Text = n.Text, Value = n.Value.ToString(), Selected = entity.MomMeetingTaskParticipant.Any(s => s.U.Uid.ToString() == n.Value), Group = n.Group }).ToList();
                        }
                        else
                        {
                            if (IsSpecialUser())
                            {
                                participants = userLoginService.GetUsersDetailsByDeptId(entity.MomMeeting.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray()).Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString(), Selected = entity.MomMeetingTaskParticipant.Any(s => s.Uid == n.Uid) }).ToList();
                            }
                            else
                            {
                                participants = userLoginService.GetUsersDetailsByDeptId(entity.MomMeeting.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray())
                                    .Where(n => (n.PMUid == CurrentUser.Uid || n.Uid == CurrentUser.Uid || n.Uid == CurrentUser.PMUid || n.PMUid == CurrentUser.PMUid))
                                    .Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString(), Selected = entity.MomMeetingTaskParticipant.Any(s => s.Uid == n.Uid) }).ToList();
                            }
                        }
                        model.PaticipantList.Clear();
                    }
                }

                //if (momMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                //{
                //    AddOtherActivityParticipants(ref participants, momMeeting.MeetingMasterID, momMeeting.CreatedDate);
                //}

                //if (!participants.Any(x => x.Value == authorParticipants.Uid.ToString()))
                //{
                //    participants.Add(new SelectListItem { Text = authorParticipants.Name, Value = authorParticipants.Uid.ToString() });
                //}

                //if (!participants.Any(x => x.Value == chairedParticipants.Uid.ToString()))
                //{
                //    participants.Add(new SelectListItem { Text = chairedParticipants.Name, Value = chairedParticipants.Uid.ToString() });
                //}

                if (momMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                {
                    participants = participants.OrderBy(o => o.Group.Name).ThenBy(O => O.Text).ToList();
                    participants.Insert(0, new SelectListItem { Text = "All Participants", Value = "0", Group = participantGroup });
                }
                else
                {
                    participants = participants.OrderBy(o => o.Text).ToList();
                    participants.Insert(0, new SelectListItem { Text = "All Participants", Value = "0" });
                }

                //participants = participants.OrderBy(o => o.Text).ToList();

                model.PaticipantList = participants;

                return PartialView("_AddEditTask", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        private bool IsSpecialUser()
        {
            return (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP ||
                                CurrentUser.RoleId == (int)Enums.UserRoles.PMO ||
                                CurrentUser.RoleId == (int)Enums.UserRoles.UKPM ||
                                CurrentUser.RoleId == (int)Enums.UserRoles.NWCloudLeadRole ||
                                 CurrentUser.RoleId == (int)Enums.UserRoles.NWHardwareSoftware ||
                                  CurrentUser.RoleId == (int)Enums.UserRoles.NWLANWAN ||
                                   CurrentUser.RoleId == (int)Enums.UserRoles.NWLeadRole ||
                                    CurrentUser.RoleId == (int)Enums.UserRoles.NWProblemResolution ||
                                CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM ||
                                CurrentUser.RoleId == (int)Enums.UserRoles.Director);
        }


        /// <summary>
        /// To add/edit task
        /// </summary>
        /// <returns> return json message either added or not. </returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        /// 

        [HttpPost]

        public ActionResult AddEditTask(MomMeetingTaskDto model, IFormFileCollection docs)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CommentByUId = CurrentUser.Uid;

                    List<string> MomTaskDocuments = new List<string>();

                    foreach (var item in docs)
                    {
                        string FileName = GeneralMethods.SaveFile(item, "Upload/MomDocument/", "");
                        model.MomMeetingTaskDocuments.Add(new MomMeetingTaskDocumentDto() { DocumentPath = FileName });
                    }
                    //foreach (var filename in model.MomMeetingTaskDocuments)
                    //{
                    //    model.MomMeetingTaskDocuments.Add(new MomMeetingTaskDocumentDto
                    //    {
                    //        AddedDate = DateTime.Now,
                    //        DocumentPath = filename.DocumentPath,
                    //    });
                    //}
                    MomMeetingTask momMeetingTask = model.Id > 0 ? MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskFindById(model.Id) : new MomMeetingTask();
                    if (momMeetingTask != null && momMeetingTask.MomMeeting != null)
                    {
                        var isEditable = (momMeetingTask.MomMeetingId == model.MomMeetingId) && (momMeetingTask.MomMeeting.AuthorByUID == CurrentUser.Uid ||
                             momMeetingTask.MomMeeting.ChairedByUID == CurrentUser.Uid) && (momMeetingTask.Status != (byte)MomMeetingStatus.Completed);
                        if (isEditable)
                        {
                            // ok
                        }
                        else
                        {
                            return MessagePartialView("You don't have permission to Add / edit MOM Action");
                        }

                    }

                    var result = MinutesOfMeetingTaskService.Save(model);
                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Minutes of Meeting Action has been {0} successfully", model.Id > 0 ? "updated" : "added")
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                else
                {
                    return CreateModelStateErrors();
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        #endregion


        #region Mom Meeting Comments
        /// <summary>
        /// To View Task Comments by Id
        /// </summary>
        /// <returns>returns partial with respective model</returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        public ActionResult ViewTaskComments(int id)
        {
            try
            {
                MomMeetingTaskCommentsDto model = new MomMeetingTaskCommentsDto();
                var momTask = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskFindById(id);
                model.MomMeetingTask.MeetingTitle = momTask.MomMeeting.MeetingTitle;
                model.MomMeetingTask.Task = momTask.Task;
                model.MomMeetingTask.Remark = momTask.Remark;
                model.MomMeetingTask.Status = (MomMeetingStatus)momTask.Status;
                model.MomMeetingTask.TargetDates = momTask.TargetDate.ToFormatDateString("dd/MM/yyyy");
                model.MomMeetingTask.PaticipantsList = string.Join(", ", momTask.MomMeetingTaskParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name));

                model.MomMeetingTaskTimeLineDto = momTask.MomMeetingTaskTimeLine.Select(x => new MomMeetingTaskTimeLineDto
                {
                    Comment = x.Comment,
                    Status = (MomMeetingStatus)x.Status,
                    CommentedBy = userLoginService.GetUserInfoByID(x.CommentByUid).Name,
                    MeetingTitle = x.MomMeeting.MeetingTitle,
                    CreatedDate = x.CreatedDate,
                    TaskCommentId = x.TaskCommentId ?? 0
                }).OrderByDescending(m => m.CreatedDate).ToList();


                return PartialView("_ViewTaskComment", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// To Add Task Comments by Id
        /// </summary>
        /// <returns>returns partial with respective model</returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        [HttpGet]
        public ActionResult AddTaskComment(int meetingId, int id)
        {
            try
            {
                MomMeetingTaskCommentsAddDto model = new MomMeetingTaskCommentsAddDto();
                var momTask = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskFindById(id);

                var momMeeting = MinutesOfMeetingService.GetMinutesOfMeetingFindById(meetingId);
                //var momMeeting = MinutesOfMeetingService.GetMinutesOfMeetingFindById(momTask.MomMeetingId);

                model.MeetingTitle = momTask.MomMeeting.MeetingTitle;
                model.Task = momTask.Task;
                model.Remark = momTask.Remark;
                model.LastComment = momTask.MomMeetingTaskTimeLine.OrderByDescending(m => m.ModifiedDate).Select(m => m.Comment).FirstOrDefault();

                model.Status = (MomMeetingStatus)momTask.Status;
                model.Priority = momTask.Priority != null ? (Enums.Priority)momTask.Priority : 0;
                /*model.TargetDates =*/
                model.TargetDate = momTask.TargetDate.ToFormatDateString("dd/MM/yyyy");
                //model.PaticipantsList = string.Join(", ", momTask.MomMeetingTaskParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name));
                //  model.PaticipantsList = string.Join(", ", momTask.MomMeetingTaskParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name));

                var participants = new List<SelectListItem>();
                var participantsMomMeetings = new List<SelectListItem>();

                //var authorParticipants = userLoginService.GetUserInfoByID(momTask.MomMeeting.AuthorByUID);
                //var chairedParticipants = userLoginService.GetUserInfoByID(momTask.MomMeeting.ChairedByUID);

                SelectListGroup participantGroup = new SelectListGroup() { Name = "A. Meeting Participants" };
                SelectListGroup otherGroup = new SelectListGroup() { Name = "B. Outside Participants" };
                if (momTask.MomMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                {
                    //participants = momTask.MomMeeting.MomMeetingParticipant.Select(n => new SelectListItem { Text = n.U.Name, Value = n.U.Uid.ToString() }).ToList();
                    //participants = momTask.MomMeeting.MomMeetingParticipant.Where(a => a.U.PMUid != null).Select(n => new SelectListItem { Text = n.U.Name + ' ' + GetPMName(n.U.PMUid.Value), Value = n.U.Uid.ToString() }).ToList();

                    var momMeetingparticipants = momMeeting.MomMeetingParticipant.Where(a => a.U.PMUid != null).Select(n => new SelectListItem { Text = n.U.Name + ' ' + (n.U.Pmu != null ? '(' + n.U.Pmu.Name + ')' : ""), Value = n.U.Uid.ToString(), Group = participantGroup }).ToList();
                    var users = userLoginService.GetUsers(true);
                    List<SelectListItem> employees = users.Where(a => a.PMUid != null).Select(n => new SelectListItem { Text = n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""), Value = n.Uid.ToString(), Group = otherGroup }).ToList();
                    employees.RemoveAll(x => momMeetingparticipants.Any(y => y.Value == x.Value));// removing meeting participant
                    participants.AddRange(momMeetingparticipants);
                    participants.AddRange(employees);

                }
                else
                {
                    if (IsSpecialUser())
                    {
                        participants = userLoginService.GetUsersDetailsByDeptId(momTask.MomMeeting.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray()).Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
                    }
                    else
                    {
                        participants = userLoginService.GetUsersDetailsByDeptId(momTask.MomMeeting.MomMeetingDepartment.Select(d => d.DepartmentId).ToArray())
                            .Where(n => (n.PMUid == CurrentUser.Uid || n.Uid == CurrentUser.Uid || n.Uid == CurrentUser.PMUid || n.PMUid == CurrentUser.PMUid))
                            .Select(n => new SelectListItem { Text = n.Name, Value = n.Uid.ToString() }).ToList();
                    }
                }

                //if (momTask.MomMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                //{
                //    AddOtherParticipants(ref participants, momMeeting.MomMeetingParticipant, momTask.MomMeetingTaskParticipant);
                //}

                //if (!participants.Any(x => x.Value == authorParticipants.Uid.ToString()))
                //{
                //    participants.Add(new SelectListItem { Text = authorParticipants.Name, Value = authorParticipants.Uid.ToString() });
                //}

                //if (!participants.Any(x => x.Value == chairedParticipants.Uid.ToString()))
                //{
                //    participants.Add(new SelectListItem { Text = chairedParticipants.Name, Value = chairedParticipants.Uid.ToString() });
                //}

                if (momTask.MomMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                {
                    participants = participants.Select(n => new SelectListItem
                    { Text = n.Text, Value = n.Value.ToString(), Selected = momTask.MomMeetingTaskParticipant.Any(s => s.U.Uid.ToString() == n.Value), Group = n.Group }
                    ).OrderBy(o => o.Group.Name).ThenBy(O => O.Text).ToList();
                    participants.Insert(0, new SelectListItem { Text = "All Participants", Value = "0", Group = participantGroup });
                }
                else
                {
                    participants = participants.Select(n => new SelectListItem { Text = n.Text, Value = n.Value.ToString(), Selected = momTask.MomMeetingTaskParticipant.Any(s => s.U.Uid.ToString() == n.Value) }).OrderBy(o => o.Text).ToList();
                    participants.Insert(0, new SelectListItem { Text = "All Participants", Value = "0" });
                }

                model.PaticipantList = participants;

                model.CommentedByUid = CurrentUser.Uid;
                model.MomMeetingId = momMeeting.Id;
                model.MomMeetingTaskId = id;
                model.Decision = momTask.Decision;
                return PartialView("_AddTaskComment", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// To Add participant of current meeting and participant that are already assigned to Task
        /// </summary>
        /// <returns>update ref variable participants.</returns>
        /// <createdby>Rahul Dev Katara</createdby>
        /// <createddate>15-10-2019</createddate>
        private void AddOtherParticipants(ref List<SelectListItem> participants, ICollection<MomMeetingParticipant> MomMeetingParticipant, ICollection<MomMeetingTaskParticipant> MomMeetingTaskParticipant)
        {
            foreach (var item in MomMeetingParticipant.Select(n => new SelectListItem { Text = n.U.Name + " " + (n.U.Pmu != null ? '(' + n.U.Pmu.Name + ')' : ""), Value = n.U.Uid.ToString() }))
            {
                if (participants.Where(e => e.Value == item.Value).Count() == 0)
                {
                    participants.Add(item);
                }
            }

            foreach (var item in MomMeetingTaskParticipant.Select(n => new SelectListItem { Text = n.U.Name + " " + (n.U.Pmu != null ? '(' + n.U.Pmu.Name + ')' : ""), Value = n.U.Uid.ToString() }))
            {
                if (participants.Where(e => e.Value == item.Value).Count() == 0)
                {
                    participants.Add(item);
                }
            }
        }

        /// <summary>
        /// To Add Task Comments with model
        /// </summary>
        /// <returns>returns partial view either updated or added.</returns>
        /// <createdby>Arbind Kumar</createdby>
        /// <createddate>09-06-2019</createddate>
        [HttpPost]
        public ActionResult AddTaskComment(MomMeetingTaskCommentsAddDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = MinutesOfMeetingTaskCommentService.Save(model);
                    if (result != null && result.Id > 0)
                    {
                        PlacePILog(result);

                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Minutes of Meeting has been {0} successfully", model.Id > 0 ? "updated" : "added")
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save record");
                    }
                }
                else
                {
                    return CreateModelStateErrors();
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        private void PlacePILog(MomMeetingTaskTimeLine comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }

            var momTask = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskFindById(comment.MomMeetingTaskId);
            var momMeeting = MinutesOfMeetingService.GetMinutesOfMeetingFindById(momTask.MomMeetingId);
            var latestMeeting = MinutesOfMeetingService.GetLatestMinutesofMeeting();
            var process = processService.GetAllProcess().FirstOrDefault(x => string.Equals(x.ProcessName, "Other", StringComparison.OrdinalIgnoreCase));
            if (string.Equals(momMeeting.MeetingMaster.Title, "SEPG Meeting", StringComparison.OrdinalIgnoreCase)
                && (comment.Status == (byte)MomMeetingStatus.Ongoing || comment.Status == (byte)MomMeetingStatus.Completed) && process != null)
            {


                var piLogRequest = new PILogRequestDto
                {
                    Id = momTask.Pilog == null ? 0 : momTask.Pilog.Id,
                    ProcessName = $"{momMeeting.MeetingMaster.Title} - {momTask.Task}",
                    PotentialArea = momTask.Decision.StripHTML(),
                    CreatedDateFromMoM = momMeeting.CreatedDate,
                    StatusFromMoM = comment.Status == (byte)MomMeetingStatus.Completed ? (byte)Enums.PILogStatus.Approved : comment.Status,
                    CurrentUserId = CurrentUser.Uid,
                    MomMeetingTaskId = momTask.Id,
                    EstimatedScheduleFromMoM = comment.Status == (byte)MomMeetingStatus.Completed ? DateTime.Now.AddDays(7) : DateTime.MinValue,
                    RemarkFromMOM = $"The decision has taken in SEPG meeting held on date {latestMeeting.DateOfMeeting.ToFormatDateString("dd MMM yyyy")}.",
                    ProcessId = process.Id
                };

                var result = PILogService.Save(piLogRequest);
            }


        }


        #endregion

        [HttpGet]
        public ActionResult ViewMeetingDecision(int masterId)
        {
            try
            {
                MomMeetingTaskDecisionDTO model = new MomMeetingTaskDecisionDTO();

                var momMeeting = MinutesOfMeetingService.GetAllMinutesOfMeetingFindByTopicId(masterId);

                model.MomMeeting = momMeeting.Select(m => new MomMeeting
                {
                    Id = m.Id,
                    MeetingTitle = m.MeetingTitle,
                    MeetingMasterID = m.MeetingMasterID,
                    DateOfMeeting = m.DateOfMeeting,
                    ChairedByUID = m.ChairedByUID,
                    AuthorByUID = m.AuthorByUID,
                    MeetingTime = m.MeetingTime,
                    VenueName = m.VenueName,
                    MomMeetingTasks = momMeeting.SelectMany(t => t.MomMeetingTasks).Select(y => new MomMeetingTask
                    {
                        Task = y.Task,
                        MomMeetingId = y.MomMeetingId,
                        Id = y.Id,
                        TargetDate = y.TargetDate,
                        //Status = (MomMeetingStatus)y.Status,
                        Remark = y.Remark,
                        Status = y.Status,
                        Decision = y.Decision,
                    }).Where(g => (g.MomMeetingId == m.Id) && (g.Decision != null)).ToList(),
                }).Where(c => c.MomMeetingTasks.Count > 0).ToList();

                return PartialView("_ViewMeetingDecision", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }

        }


        /// <summary>
        /// To Add Task Decision by Id
        /// </summary>
        /// <returns>returns partial with respective model</returns>
        /// <createdby>Apoorva Joshi</createdby>
        /// <createddate>16-07-2019</createddate>
        [HttpGet]
        public ActionResult AddTaskDecision(int id)
        {
            try
            {
                MomMeetingTaskDto model = new MomMeetingTaskDto();
                var momTask = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskFindById(id);

                model.Id = id;
                model.Decision = momTask.Decision;
                return PartialView("_AddTaskDecision", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// To Add Task Decision with model
        /// </summary>
        /// <returns>returns partial view either updated or added.</returns>
        /// <createdby>Apoorva Joshi</createdby>
        /// <createddate>09-06-2019</createddate>
        [HttpPost]
        public ActionResult AddTaskDecision(MomMeetingTaskDto model)
        {
            try
            {
                if (model.Id != 0)
                {
                    var result = MinutesOfMeetingTaskService.SaveTaskDecision(model);
                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Minutes of Meeting task decision has been updated successfully")
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to save decision");
                    }

                }
                else
                {
                    return CreateModelStateErrors();
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// To Delete task by Id
        /// </summary>
        /// <returns>returns partial with respective model</returns>
        /// <createdby>Apoorva Joshi</createdby>
        /// <createddate>16-07-2019</createddate>
        [HttpGet]
        public ActionResult DeleteTask(int id)
        {
            try
            {
                if (id > 0)
                {
                    MomMeetingTask momMeetingTask = id > 0 ? MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskFindById(id) : new MomMeetingTask();
                    if (momMeetingTask != null && momMeetingTask.MomMeeting != null)
                    {
                        var isEditable = (momMeetingTask.MomMeeting.AuthorByUID == CurrentUser.Uid ||
                             momMeetingTask.MomMeeting.ChairedByUID == CurrentUser.Uid) && (momMeetingTask.Status != (byte)MomMeetingStatus.Completed);
                        if (isEditable)
                        {
                            // ok
                        }
                        else
                        {
                            return MessagePartialView("You don't have permission to delete this MOM Action");
                        }

                    }
                }
                else
                {
                    return MessagePartialView("Id is incorrect");
                }


                MomMeetingTaskDto model = new MomMeetingTaskDto();
                return PartialView("_DeleteTask", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// To Delete task by Id
        /// </summary>
        /// <returns>returns partial view either updated or added.</returns>
        /// <createdby>Apoorva Joshi</createdby>
        /// <createddate>16-07-2019</createddate>
        [HttpPost]
        public ActionResult DeleteTask(MomMeetingTaskDto model)
        {
            try
            {
                if (model.Id != 0)
                {
                    bool result = MinutesOfMeetingTaskService.Delete(model.Id);
                    if (result)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Minutes of Meeting task has been deleted successfully")
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unable to delete record");
                    }

                }
                else
                {
                    return CreateModelStateErrors();
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        public ActionResult ExportToExcel(int masterId)
        {
            if (masterId != 0)
            {
                var response = MinutesOfMeetingMasterService.GetMinutesOfMeetingFindById(masterId);

                if (response != null)
                {
                    List<MomMeeting> responseMOM = new List<MomMeeting>();
                    responseMOM = response.MomMeetings.ToList();
                    responseMOM = responseMOM.OrderByDescending(m => m.CreatedDate).ToList();

                    var meetingsReport = responseMOM.Select((mom, index) => new
                    {
                        Id = mom.Id,
                        MeetingTitle = mom.MeetingTitle,
                        MeetingMasterId = mom.MeetingMasterID,
                        Aganda = mom.Agenda,
                        venue = mom.VenueName,
                        DateOfMeeting = $"{mom.DateOfMeeting.ToFormatDateString("MMM d, yyyy")}&nbsp;({mom.MeetingTime} &nbsp;min)",
                        AuthorBy = userLoginService.GetUserInfoByID(mom.AuthorByUID).Name,
                        AuthorByUid = mom.AuthorByUID,
                        ChairedByUid = mom.ChairedByUID,
                        ChairedBy = userLoginService.GetUserInfoByID(mom.ChairedByUID).Name,
                        Participants = string.Join(", ", mom.ParticipantType == (byte)MomMeetingParticipantType.Individual ? mom.MomMeetingParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name) : mom.MomMeetingDepartment.Select(d => DepartmentService.GetDepartmentById(d.DepartmentId).Name)),
                        resTaskList = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskList(mom.Id),
                    }).ToList();

                    List<ExportExcelData> dataToExport = new List<ExportExcelData>();

                    dataToExport.Add(new ExportExcelData
                    {
                        Heading = "Meetings",
                        ShowSrNo = true,
                        ColumnsToTake = new string[] { "MeetingTitle", "venue", "DateOfMeeting", "Participants", "AuthorBy", "ChairedBy", "Aganda" },
                        DataTable = ExportExcelHelper.ToDataTable(meetingsReport)
                    });

                    byte[] filecontent = ExportExcelHelper.ExportExcel(dataToExport);
                    string fileName = $"MinutesOfMeetingReport_{DateTime.Now.Ticks}.xlsx";
                    return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
                }
                else
                {
                    return Content("Unable to get filters");
                }
            }
            else
            {
                return Content("Unable to get filters");
            }
        }

        [HttpGet]
        public ActionResult SendMinutesOfMeetingEmail(int? id)
        {
            MomMeetingDto model = new MomMeetingDto();
            if (id != null && id != 0)
            {
                var users = userLoginService.GetUsers(true);
                var Department = DepartmentService.GetActiveDepartments();

                var entity = MinutesOfMeetingService.GetMinutesOfMeetingFindById(id.Value);

                model.MeetingMasterID = entity.MeetingMasterID;
                model.ParticipantType = (MomMeetingParticipantType)entity.ParticipantType;

                foreach (var item in entity.MomMeetingParticipant)
                {
                    model.selectedParticpants += item.U.EmailOffice + ";";
                    // model.selectedParticpantsForEmails += item.U.EmailOffice + "_" + item.U.Name + ",";
                }
                if (entity.MomMeetingParticipant.Count() > 0)
                {
                    //var otherParticipants = entity.MomMeetingTasks.SelectMany(m => m.MomMeetingTaskParticipant.Select(p => p.U.EmailOffice)).ToArray();
                    var otherParticipants = entity.MomMeetingTasks.SelectMany(m => m.MomMeetingTaskParticipant.Select(p => p)).ToList();
                    otherParticipants.RemoveAll(o => entity.MomMeetingParticipant.Any(p => p.Uid == o.Uid));
                    if (otherParticipants != null && otherParticipants.Count() > 0 && entity.MomMeetingParticipant.Count() > 0)
                    {
                        foreach (var item in otherParticipants)
                        {
                            if (!model.selectedParticpants.Contains(item.U.EmailOffice)) // same other participant may be in two tasks
                            {
                                model.selectedParticpants += item.U.EmailOffice + ";";
                            }
                        }
                    }

                    model.selectedParticpants = model.selectedParticpants.ToString().TrimEnd(';');
                }
                foreach (var item in entity.MomMeetingDepartment)
                {
                    model.selectedGroup += item.Department.Name + ";";
                }
                if (entity.MomMeetingDepartment.Count > 0)
                {
                    model.selectedGroup = model.selectedGroup.ToString().TrimEnd(';');
                }

                model.PaticipantList = users.Select(n => new SelectListItem { Text = n.EmailOffice, Value = n.Uid.ToString(), Selected = entity.MomMeetingParticipant.Any(s => s.Uid == n.Uid) }).ToList();
                model.GroupList = Department.Select(n => new SelectListItem { Text = n.Name, Value = n.DeptId.ToString(), Selected = entity.MomMeetingDepartment.Any(d => d.DepartmentId == n.DeptId) }).ToList();
            }


            model.Id = id.Value;

            return PartialView("_SendEmail", model);
        }


        [HttpPost]
        public ActionResult SendMinutesOfMeetingEmail(MomMeetingDto model)
        {
            try
            {
                if (model.selectedGroup == null && model.selectedParticpants == null)
                {
                    return MessagePartialView("Please select at least one group or participants.");
                }

                var momMeeting = MinutesOfMeetingService.GetMinutesOfMeetingFindById(model.Id);

                var MomMeetingTask = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskForEmail(model.MeetingMasterID, momMeeting.CreatedDate).OrderBy(c => c.CreatedDate).OrderByDescending(c => c.ModifiedDate).ToList();



                if (momMeeting.Id > 0)
                {
                    #region

                    FlexiMail objSendMail = new FlexiMail();

                    string fromemail = CurrentUser.EmailOffice;
                    string userName = CurrentUser.Name;

                    objSendMail.From = fromemail;

                    var participants = new List<SelectListItem>();

                    var momMeetingParticipant = new List<MomMeetingParticipant>();
                    var momMeetingDepartment = new List<MomMeetingDepartment>();
                    string[] strSelectedParticpants = new string[] { };
                    strSelectedParticpants = model.selectedParticpants != null ? model.selectedParticpants.Split(";") : null;
                    var users = userLoginService.GetUsers(true);
                    model.PaticipantList = users.Where(x => !string.IsNullOrWhiteSpace(x.EmailOffice)).Select(n => new SelectListItem { Text = n.EmailOffice, Value = n.Uid.ToString() }).ToList();


                    if (momMeeting.ParticipantType == (byte)MomMeetingParticipantType.Group)
                    {

                        string[] strSelectedDepartments = new string[] { };
                        strSelectedDepartments = model.selectedGroup != null ? model.selectedGroup.Split(",") : null;

                        var Department = DepartmentService.GetActiveDepartments();
                        model.GroupList = Department.Select(n => new SelectListItem { Text = n.Name, Value = n.DeptId.ToString() }).ToList();

                        if (strSelectedDepartments != null)
                        {
                            for (int i = 0; i < strSelectedDepartments.Length; i++)
                            {
                                if (strSelectedDepartments[i] != string.Empty)
                                {
                                    int[] otherDepartment = Department.Select(n => new SelectListItem { Text = n.Name.Trim(), Value = n.DeptId.ToString() }).Where(m => m.Text == strSelectedDepartments[i].Trim().ToString()).Select(n => Convert.ToInt32(n.Value)).ToArray();
                                    if (otherDepartment.Length > 0)
                                    {
                                        var dept = Department.Where(m => m.DeptId == Convert.ToInt32(otherDepartment[0].ToString())).FirstOrDefault();
                                        momMeetingDepartment.Add(new MomMeetingDepartment { DepartmentId = dept.DeptId, Department = dept, MomMeeting = momMeeting, MomMeetingId = momMeeting.Id });

                                        //participants = userLoginService.GetUsersDetailsByDeptId(momMeetingDepartment.Select(d => d.DepartmentId).ToArray()).Select(n => new SelectListItem { Text = n.EmailOffice, Value = n.Uid.ToString() }).ToList();

                                        if (IsSpecialUser())
                                        {
                                            participants = userLoginService.GetUsersDetailsByDeptId(momMeetingDepartment.Select(d => d.DepartmentId).ToArray()).Where(x => !string.IsNullOrWhiteSpace(x.EmailOffice)).Select(n => new SelectListItem { Text = n.EmailOffice, Value = n.Uid.ToString() }).ToList();
                                        }
                                        else
                                        {
                                            participants = userLoginService.GetUsersDetailsByDeptId(momMeetingDepartment.Select(d => d.DepartmentId).ToArray())
                                                .Where(n => (n.PMUid == CurrentUser.Uid || n.Uid == CurrentUser.Uid || n.PMUid == CurrentUser.PMUid) && !string.IsNullOrWhiteSpace(n.EmailOffice))
                                                .Select(n => new SelectListItem { Text = n.EmailOffice, Value = n.Uid.ToString() }).ToList();
                                        }
                                    }

                                }
                            }
                        }
                    }
                    else if (momMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                    {

                        string[] mparticipants = momMeeting.MomMeetingParticipant.Select(p => p.U.EmailOffice).ToArray();
                        //mparticipants=strSelectedParticpants.Where(s => mparticipants.Any(m => string.Equals(m,s) m.Equals(s))).ToArray();// only participant that are selected
                        mparticipants = strSelectedParticpants.Where(s => mparticipants.Any(m => string.Equals(m, s, StringComparison.CurrentCultureIgnoreCase))).ToArray();// only participant that are selected

                        strSelectedParticpants = mparticipants;
                    }

                    if (strSelectedParticpants != null)
                    {
                        for (int i = 0; i < strSelectedParticpants.Length; i++)
                        {
                            if (strSelectedParticpants[i] != string.Empty)
                            {
                                string otherParticipantsUid = model.PaticipantList.Where(m => m.Text.Trim() == strSelectedParticpants[i].Trim().ToString()).Select(n => n.Value).FirstOrDefault();

                                if (otherParticipantsUid != string.Empty && otherParticipantsUid != null)
                                {
                                    var taskUserDto = users.Where(m => m.Uid.ToString() == otherParticipantsUid).FirstOrDefault();
                                    momMeetingParticipant.Add(new MomMeetingParticipant { MomMeeting = momMeeting, MomMeetingId = momMeeting.Id, U = taskUserDto, Uid = taskUserDto.Uid });
                                    participants.Add(new SelectListItem { Text = strSelectedParticpants[i].Trim().ToString(), Value = otherParticipantsUid.ToString() });
                                }
                                else
                                {
                                    participants.Add(new SelectListItem { Text = strSelectedParticpants[i].Trim().ToString(), Value = "" });
                                }

                            }
                        }
                    }

                    StringBuilder builder = new StringBuilder();
                    //var momMeetingFilterTask = MomMeetingTask.Where(m => m.MomMeetingTaskTimeLine.Count > 0 ? m.MomMeetingTaskTimeLine.OrderByDescending(n => n.CreatedDate).FirstOrDefault().CreatedDate >= momMeeting.DateOfMeeting && m.MomMeetingTaskTimeLine.OrderByDescending(n => n.CreatedDate).FirstOrDefault().MomMeetingId == momMeeting.Id : false);
                    var momMeetingFilterTask = MomMeetingTask.Where(m => m.MomMeetingTaskTimeLine.Count > 0 ? m.MomMeetingTaskTimeLine.OrderByDescending(n => n.CreatedDate).FirstOrDefault().CreatedDate >= momMeeting.DateOfMeeting && m.MomMeetingTaskTimeLine.OrderByDescending(n => n.CreatedDate).Any(x => x.MomMeetingId == momMeeting.Id) : false).ToList();

                    if (momMeetingFilterTask.Count() > 0)
                    {
                        builder.Append("<tr>");
                        builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:15px;border-bottom:1px solid #cccccc;\"><strong> Please go through the below Listed MOM action points:</strong></td>");
                        builder.Append("</tr>");

                        foreach (var item in momMeetingFilterTask)
                        {
                            string taskParticipantsName = string.Empty;
                            var resTaskParticipants = item.MomMeetingTaskParticipant.Where(m => m.MomMeetingTaskId == item.Id).ToList();
                            var comment = item.MomMeetingTaskTimeLine.Count > 0 ? item.MomMeetingTaskTimeLine.OrderByDescending(m => m.CreatedDate).FirstOrDefault().Comment : "";
                            //if (comment != null && comment != string.Empty)
                            //{
                            builder.Append("<tr>");
                            builder.Append("<td valign=\"top\" style=\"padding-bottom:25px;padding-top:15px;\">");
                            builder.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-bottom:1px solid #cccccc; padding-bottom:10px;table-layout:fixed;\">");
                            builder.Append("<tr>");
                            builder.Append("<td width=\"12px\" style=\"font-size:24px; line-height:0;mso-line-height-rule: exactly; vertical-align:top;\"><span style=\"display:block;margin-top:10px;\">&bull;</span></td>");
                            builder.Append("<td width=\"10px\"></td>");
                            builder.Append("<td valign=\"top\" style=\"padding-bottom:5px;\">");
                            builder.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
                            builder.Append("<tr>");
                            builder.Append("<td style=\"padding-bottom:10px;\">");
                            builder.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
                            builder.Append("<tr>");

                            builder.Append("<td style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:bold; padding-right:10px;\">" + item.Task + "");
                            if (item.Status == (byte)MomMeetingStatus.Ongoing)
                            {
                                builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:#25ad25; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)item.Status + ") </span>");
                            }
                            else if (item.Status == (byte)MomMeetingStatus.Delayed)
                            {
                                builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:#e4305a; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)item.Status + ") </span>");
                            }
                            else if (item.Status == (byte)MomMeetingStatus.Completed)
                            {
                                builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:darkgreen; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)item.Status + ") </span>");
                            }

                            builder.Append("</td>");


                            builder.Append("</tr>");

                            builder.Append("</table>");

                            builder.Append("</td>");

                            builder.Append("</tr>");

                            foreach (var taskParticipants in resTaskParticipants)
                            {
                                taskParticipantsName += taskParticipants.U.Name + ", ";
                            }
                            if (taskParticipantsName.EndsWith(", "))
                            {
                                taskParticipantsName = taskParticipantsName.Remove(taskParticipantsName.Length - 2);
                            }

                            if (taskParticipantsName != string.Empty)
                            {
                                builder.Append("<tr><td style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; text-decoration:underline; padding-right:7px;padding-bottom:10px;\"> " + taskParticipantsName + " </td></tr>");
                            }

                            builder.Append("<tr>");

                            builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\">" + item.Remark + "</td>");
                            builder.Append("</tr>");
                            builder.Append("<tr>");
                            builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"><strong> Target Date: " + item.TargetDate.ToFormatDateString("MMM d, yyyy") + " </strong></td>");
                            builder.Append("</tr>");

                            if (comment != null && comment != string.Empty)
                            {
                                builder.Append("<tr>");
                                builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"><strong> Comment:</strong> " + comment + "</td>");
                                builder.Append("</tr>");
                            }

                            if (item.MomMeetingTaskDocuments.Count > 0)
                            {
                                builder.Append("<tr>");
                                builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"><strong>Please check below files from attached documents:</strong></td>");
                                builder.Append("</tr>");
                                for (int i = 1; i <= item.MomMeetingTaskDocuments.Count; i++)
                                {
                                    MomMeetingTaskDocument itemTaskDoc = (MomMeetingTaskDocument)item.MomMeetingTaskDocuments.ElementAt(i - 1);
                                    string filePathForActionDoc = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", itemTaskDoc.DocumentPath);
                                    if (System.IO.File.Exists(filePathForActionDoc))
                                    {
                                        builder.Append("<tr>");
                                        builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\">" + i + ") " + itemTaskDoc.DocumentPath + "</td>");
                                        builder.Append("</tr>");
                                    }
                                }
                            }


                            builder.Append("</table></td>");

                            builder.Append("</tr>");
                            builder.Append("</table></td>");
                            builder.Append("</tr>");



                            //builder.Append("<tr>");
                            //builder.Append("<td valign=\"top\" style=\"padding-left:25px; padding-bottom:25px;\">");
                            //builder.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-bottom:1px solid #cccccc; padding-bottom:10px;\">");
                            //builder.Append("<tr>");
                            //builder.Append("<td valign=\"top\" style=\"padding-bottom:10px;\">");
                            //builder.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
                            //builder.Append("<tr>");
                            ////builder.Append("<td style=\"margin-top:0px;\" width=\"5\"><img src=\"" + SiteKey.DomainName + "Content/images/black-dot.png\" width =\"5\" height=\"5\" alt=\"\"/></td>");
                            //builder.Append("<td style=\"margin-top:0px;\" width=\"5\">&bull;</td>");
                            //builder.Append("<td style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:bold; padding-right:10px; padding-left:8px;padding-bottom:10px;\"> " + item.Task + "");
                            //if (item.Status == (byte)MomMeetingStatus.Ongoing)
                            //    builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:#25ad25; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)item.Status + ") </span>");
                            //else if (item.Status == (byte)MomMeetingStatus.Delayed)
                            //    builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:#e4305a; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)item.Status + ") </span>");
                            //else if (item.Status == (byte)MomMeetingStatus.Completed)
                            //    builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:darkgreen; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)item.Status + ") </span>");

                            //builder.Append("</td>");
                            //builder.Append("</tr>");
                            //builder.Append("<tr>");
                            //foreach (var taskParticipants in resTaskParticipants)
                            //{
                            //    taskParticipantsName += taskParticipants.U.Name + ", ";
                            //}
                            //if (taskParticipantsName.EndsWith(", "))
                            //    taskParticipantsName = taskParticipantsName.Remove(taskParticipantsName.Length - 2);
                            //if (taskParticipantsName != string.Empty)
                            //    builder.Append("<td colspan=\"3\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; text-decoration:underline; padding-right:7px;\"> " + taskParticipantsName + " </td>");

                            //builder.Append("</tr>");
                            //builder.Append("</table>");
                            //builder.Append("</td>");
                            //builder.Append("</tr>");
                            //builder.Append("<tr>");
                            //builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"> " + item.Remark + "</td>");
                            //builder.Append("</tr>");
                            //builder.Append("<tr>");
                            //builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"><strong> Target Date: " + item.TargetDate.ToFormatDateString("dd/MM/yyyy") + " </strong></td>");
                            //builder.Append("</tr>");
                            //builder.Append("<tr>");
                            //builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"><strong> Comment:</strong> " + comment + " </td>");
                            //builder.Append("</tr>");
                            //builder.Append("</table>");
                            //builder.Append("</td>");
                            //builder.Append("</tr>");

                            //builder.Append("<li>");
                            //builder.Append("<strong> " + item.Task + "</strong> - ");
                            //foreach (var taskParticipants in resTaskParticipants)
                            //{
                            //    taskParticipantsName += taskParticipants.U.Name + ", ";
                            //}

                            //if (taskParticipantsName.EndsWith(", "))
                            //    taskParticipantsName = taskParticipantsName.Remove(taskParticipantsName.Length - 2);
                            //if (taskParticipantsName != string.Empty)
                            //    builder.Append("<span style=\"padding:8px 0; text-decoration:underline\"> " + taskParticipantsName + "</span>");


                            //if (item.Status == (byte)MomMeetingStatus.Ongoing)
                            //    builder.Append("<span style=\"color:#25ad25;font-size:13px; line-height: 25px; padding:8px 0;\"> (" + (Enums.MomMeetingStatus)item.Status + ")</span>");
                            //else if (item.Status == (byte)MomMeetingStatus.Delayed)
                            //    builder.Append("<span style=\"color:#e4305a;font-size:13px; line-height: 25px; padding:8px 0;\"> (" + (Enums.MomMeetingStatus)item.Status + ") </span>");
                            //else if (item.Status == (byte)MomMeetingStatus.Completed)
                            //    builder.Append("<span valign=\"top\" style=\"color:darkgreen;font-size:13px; line-height: 25px; padding:8px 0;\"> (" + (Enums.MomMeetingStatus)item.Status + ") </span>");

                            //builder.Append("<br/><span style=\"font-size:13px; line-height: 25px;\">" + item.Remark + "</span><br/><br/><span style=\"font-size:13px;padding-top:4px;padding-bottom:4px;\"><strong>Target Date : " + item.TargetDate.ToFormatDateString("dd/MM/yyyy") + "</strong></span><br/><br/><span style=\"font-size:13px; line-height: 25px;\"><strong>Comment </strong> :  " + comment + "</span><br/>");
                            //builder.Append("</li><br />");
                            // }
                        }

                        //builder.Append("</ul>");
                        //builder.Append("</td></tr>");
                    }


                    StringBuilder participantsbuilder = new StringBuilder();
                    string partcipantsName = string.Empty;
                    if (momMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                    {
                        if (momMeeting.MomMeetingParticipant.Count > 0)
                        {
                            foreach (var item in momMeeting.MomMeetingParticipant)
                            {
                                partcipantsName += item.U.Name + ", ";
                            }
                        }
                    }
                    else
                    {
                        if (momMeeting.MomMeetingDepartment.Count > 0)
                        {
                            foreach (var item in momMeeting.MomMeetingDepartment)
                            {
                                partcipantsName += item.Department.Name + ", ";
                            }
                        }
                    }

                    if (partcipantsName.EndsWith(", "))
                    {
                        partcipantsName = partcipantsName.Remove(partcipantsName.Length - 2);
                    }

                    if (partcipantsName != string.Empty)
                    {
                        if (model.ParticipantType == Enums.MomMeetingParticipantType.Individual)
                        {
                            participantsbuilder.Append("Attendees :");
                        }
                        else
                        {
                            participantsbuilder.Append("Department :");
                        }
                    }

                    objSendMail.ValueArray = new string[] {
                                                           momMeeting.MeetingTitle != null ? "<b>" + momMeeting.MeetingTitle +"</b>" : "",
                                                            momMeeting.DateOfMeeting.ToFormatDateString("MMM d, yyyy"),
                                                            momMeeting.MeetingTime.ToString(),
                                                            momMeeting.Agenda != null ? momMeeting.Agenda : "No Description",
                                                            momMeeting.VenueName != null ? momMeeting.VenueName : "",
                                                            participantsbuilder.ToString(),
                                                            partcipantsName.ToString(),
                                                            MomMeetingTask.Count() > 0 ? builder.ToString() : "",
                                                            CurrentUser.Name
                                                        };
                    objSendMail.MailBody = objSendMail.GetHtml("MeetingEmail.html");
                    objSendMail.Subject = (momMeeting.MeetingTitle != null ? "MOM : " + momMeeting.MeetingMaster.Title + " - " + momMeeting.MeetingTitle : "null ");


                    string sendMailTo = string.Empty;
                    foreach (var item in participants)
                    {
                        sendMailTo += item.Text + ";";
                    }

                    if (sendMailTo != string.Empty)
                    {
                        objSendMail.To = sendMailTo.ToString().TrimEnd(';');
                    }

                    objSendMail.MailBodyManualSupply = true;


                    List<string> MomDocuments = new List<string>();
                    foreach (var item in momMeeting.Momdocuments)
                    {
                        string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", item.DocumentPath);
                        if (System.IO.File.Exists(FilePath))
                        {
                            MomDocuments.Add(FilePath);
                        }
                    }

                    foreach (var itemMomTask in momMeeting.MomMeetingTasks)
                    {
                        foreach (var itemTaskDoc in itemMomTask.MomMeetingTaskDocuments)
                        {
                            string filePathForActionDoc = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", itemTaskDoc.DocumentPath);
                            if (System.IO.File.Exists(filePathForActionDoc))
                            {
                                MomDocuments.Add(filePathForActionDoc);
                            }
                        }
                    }

                    if (MomDocuments.Count > 0)
                    {
                        objSendMail.AttachFile = MomDocuments.ToArray();
                    }

                    try
                    {
                        objSendMail.Send();
                        if (momMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                        {
                            SendMOMEmailOtherParticipants(model);
                        }

                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Mail Sent successfully")
                        });

                    }
                    catch (Exception ex)
                    {

                        return MessagePartialView("Please enter valid email id.");
                    }


                    #endregion
                }
                else
                {
                    return MessagePartialView("Unable to save record");
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        public ActionResult SendMinutesOfMeetingEmailToParticipants(MomMeeting momMeeting, List<string> files)
        {
            try
            {
                //if (model.selectedGroup == null && model.selectedParticpants == null)
                //{
                //    return MessagePartialView("Please select at least one group or participants..");
                //}

                //var momMeeting = MinutesOfMeetingService.GetMinutesOfMeetingFindById(model.Id);

                if (momMeeting.Id > 0)
                {
                    #region
                    string participants = string.Empty;

                    FlexiMail objSendMail = new FlexiMail();

                    string fromemail = CurrentUser.EmailOffice;
                    string userName = CurrentUser.Name;

                    objSendMail.From = fromemail;

                    DateTime time = DateTime.Today.Add(momMeeting.MeetingStartTime.Value);
                    string displayTime = time.ToString("hh:mm tt");

                    objSendMail.ValueArray = new string[] {
                                                           momMeeting.MeetingTitle != null ? "<b>" + momMeeting.MeetingTitle +"</b>" : "",
                                                           momMeeting.VenueName != null ? momMeeting.VenueName : "",
                                                            momMeeting.DateOfMeeting.ToFormatDateString("MMM d, yyyy"),
                                                            momMeeting.Agenda != null ? momMeeting.Agenda : "General Meeting",
                                                            momMeeting.Notes !=null?momMeeting.Notes:" N/A",
                                                             momMeeting.MeetingStartTime != null ? displayTime :"",
                                                              Convert.ToString(momMeeting.MeetingTime),
                                                              CurrentUser.Name,
                                                        };
                    objSendMail.MailBody = objSendMail.GetHtml("MeetingEmailToParticipant.html");
                    objSendMail.Subject = (momMeeting.MeetingTitle != null ? "Meeting Schedule : " + momMeeting.MeetingMaster.Title + " - " + momMeeting.MeetingTitle : "null ");


                    string sendMailTo = string.Empty;


                    if (momMeeting.ParticipantType == (byte)Enums.MomMeetingParticipantType.Individual)
                    {
                        if (momMeeting.MomMeetingParticipant != null && momMeeting.MomMeetingParticipant.Count > 0)
                        {
                            participants = string.Join(";", momMeeting.MomMeetingParticipant.Select(P => P.U.EmailOffice));
                            if (!string.IsNullOrWhiteSpace(participants))
                            {
                                objSendMail.To = participants;
                            }
                        }
                    }
                    else if (momMeeting.ParticipantType == (byte)Enums.MomMeetingParticipantType.Group)
                    {

                        if (momMeeting.MomMeetingDepartment != null && momMeeting.MomMeetingDepartment.Count > 0)
                        {

                            if (IsSpecialUser())
                            {
                                participants = string.Join(";", momMeeting.MomMeetingDepartment.SelectMany(x => x.Department.UserLogin.Select(y => y.EmailOffice)));
                            }
                            else
                            {
                                participants = string.Join(";",
                                    momMeeting.MomMeetingDepartment.SelectMany(x => x.Department.UserLogin
                                    .Where(u => u.IsActive == true && (u.PMUid == CurrentUser.Uid || u.Uid == CurrentUser.Uid || u.PMUid == CurrentUser.PMUid))
                                    .Select(y => y.EmailOffice)));
                            }



                            if (!string.IsNullOrWhiteSpace(participants))
                            {
                                objSendMail.To = participants;
                            }
                        }

                    }

                    objSendMail.MailBodyManualSupply = true;

                    try
                    {
                        if (files.Count > 0)
                        {
                            objSendMail.AttachFile = files.ToArray();
                        }


                        objSendMail.Send();

                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = string.Format("Mail Sent successfully")
                        });

                    }
                    catch (Exception)
                    {

                        return MessagePartialView("Please enter valid email id.");
                    }


                    #endregion
                }
                else
                {
                    return MessagePartialView("Unable to save record");
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPost]
        public JsonResult DeleteDocument(int id)
        {
            var status = false;
            var document = MinutesOfMeetingService.GetDocument(id);
            if (document != null)
            {
                string filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", document.DocumentPath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);

                }
                status = MinutesOfMeetingService.DeleteDocument(id);
            }

            return Json(status);
        }

        [HttpPost]
        public JsonResult DeleteMomMeetingTaskDocument(int id)
        {
            var status = false;
            var document = MinutesOfMeetingService.GetMomMeetingTaskDocument(id);
            if (document != null)
            {
                string filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", document.DocumentPath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);

                }
                status = MinutesOfMeetingService.DeleteMomMeetingTaskDocument(id);
            }

            return Json(status);
        }

        //[HttpGet]
        private string GetPMName(int id)
        {
            var user = string.Empty;
            var userdata = userLoginService.GetUserInfoByID(id);
            if (userdata != null)
            {
                user = '(' + userdata.Name + ')';
            }
            else
            {
                user = "";
            }

            return user;
        }

        private void SendMOMEmailOtherParticipants(MomMeetingDto model)
        {
            try
            {
                var momMeeting = MinutesOfMeetingService.GetMinutesOfMeetingFindById(model.Id);

                var MomMeetingTask = MinutesOfMeetingTaskService.GetMinutesOfMeetingTaskForEmail(model.MeetingMasterID, momMeeting.CreatedDate).OrderBy(c => c.CreatedDate).OrderByDescending(c => c.ModifiedDate);



                if (momMeeting.Id > 0)
                {
                    #region

                    FlexiMail objSendMail = new FlexiMail();

                    string fromemail = CurrentUser.EmailOffice;
                    string userName = CurrentUser.Name;

                    objSendMail.From = fromemail;

                    var participants = new List<SelectListItem>();

                    var momMeetingParticipant = new List<MomMeetingParticipant>();
                    //var momMeetingDepartment = new List<MomMeetingDepartment>();
                    string[] strSelectedParticpants = new string[] { };
                    strSelectedParticpants = model.selectedParticpants != null ? model.selectedParticpants.Split(";") : null;
                    //var users = userLoginService.GetUsers(true);
                    //model.PaticipantList = users.Where(x => !string.IsNullOrWhiteSpace(x.EmailOffice)).Select(n => new SelectListItem { Text = n.EmailOffice, Value = n.Uid.ToString() }).ToList();

                    List<MomMeetingTaskParticipant> otherTaskPartcipants = null;

                    if (momMeeting.MomMeetingParticipant.Count() > 0)
                    {

                        var allTaskParticipantParticipants = momMeeting.MomMeetingTasks.SelectMany(m => m.MomMeetingTaskParticipant.Select(p => p)).ToList();
                        allTaskParticipantParticipants.RemoveAll(o => momMeeting.MomMeetingParticipant.Any(p => p.Uid == o.Uid)); //Removing all meeting participants
                        if (allTaskParticipantParticipants != null && allTaskParticipantParticipants.Count() > 0 && momMeeting.MomMeetingParticipant.Count() > 0)
                        {
                            otherTaskPartcipants = allTaskParticipantParticipants.Where(op => op.U.EmailOffice != string.Empty
                            && strSelectedParticpants.Any(p => p.ToLower().Equals(op.U.EmailOffice.ToLower()))).GroupBy(x => x.Uid).Select(y => y.First()).ToList();
                        }
                    }


                    //StringBuilder builder = new StringBuilder();

                    var momMeetingFilterTask = MomMeetingTask.Where(m => m.MomMeetingTaskTimeLine.Count > 0 ? m.MomMeetingTaskTimeLine.OrderByDescending(n => n.CreatedDate).FirstOrDefault().CreatedDate >= momMeeting.DateOfMeeting && m.MomMeetingTaskTimeLine.OrderByDescending(n => n.CreatedDate).FirstOrDefault().MomMeetingId == momMeeting.Id : false);
                    //int[] tParticipants=null;
                    //List<MomMeetingTaskParticipant> tParticipants = null;
                    //if (otherTaskPartcipants != null)
                    //{
                    //    //tParticipants = otherTaskPartcipants.Select(p => p.Uid).Distinct().ToArray();
                    //    tParticipants = otherTaskPartcipants.Where(op=>op.U.EmailOffice!=string.Empty).Distinct().ToList();
                    //}

                    if (momMeetingFilterTask.Count() > 0 && otherTaskPartcipants != null)
                    {

                        foreach (var taskParticipant in otherTaskPartcipants)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.Append("<tr>");
                            builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:15px;border-bottom:1px solid #cccccc;\"><strong> Please go through the below Listed MOM action points:</strong></td>");
                            builder.Append("</tr>");


                            foreach (var task in momMeetingFilterTask)
                            {
                                //if(task.MomMeetingTaskParticipant.Any(p=>p.U.Uid== taskParticipant))
                                if (task.MomMeetingTaskParticipant.Any(p => p.U.Uid == taskParticipant.U.Uid))
                                {

                                    string taskParticipantsName = string.Empty;
                                    var resTaskParticipants = task.MomMeetingTaskParticipant.Where(m => m.MomMeetingTaskId == task.Id).ToList();
                                    var comment = task.MomMeetingTaskTimeLine.Count > 0 ? task.MomMeetingTaskTimeLine.OrderByDescending(m => m.CreatedDate).FirstOrDefault().Comment : "";
                                    builder.Append("<tr>");
                                    builder.Append("<td valign=\"top\" style=\"padding-bottom:25px;padding-top:15px;\">");
                                    builder.Append("<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-bottom:1px solid #cccccc; padding-bottom:10px;table-layout:fixed;\">");
                                    builder.Append("<tr>");
                                    builder.Append("<td width=\"12px\" style=\"font-size:24px; line-height:0;mso-line-height-rule: exactly; vertical-align:top;\"><span style=\"display:block;margin-top:10px;\">&bull;</span></td>");
                                    builder.Append("<td width=\"10px\"></td>");
                                    builder.Append("<td valign=\"top\" style=\"padding-bottom:5px;\">");
                                    builder.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
                                    builder.Append("<tr>");
                                    builder.Append("<td style=\"padding-bottom:10px;\">");
                                    builder.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
                                    builder.Append("<tr>");

                                    builder.Append("<td style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:bold; padding-right:10px;\">" + task.Task + "");
                                    if (task.Status == (byte)MomMeetingStatus.Ongoing)
                                    {
                                        builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:#25ad25; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)task.Status + ") </span>");
                                    }
                                    else if (task.Status == (byte)MomMeetingStatus.Delayed)
                                    {
                                        builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:#e4305a; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)task.Status + ") </span>");
                                    }
                                    else if (task.Status == (byte)MomMeetingStatus.Completed)
                                    {
                                        builder.Append("<span style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; color:darkgreen; padding-right:10px;\"> (" + (Enums.MomMeetingStatus)task.Status + ") </span>");
                                    }

                                    builder.Append("</td>");


                                    builder.Append("</tr>");

                                    builder.Append("</table>");

                                    builder.Append("</td>");

                                    builder.Append("</tr>");

                                    foreach (var taskParticipants in resTaskParticipants)
                                    {
                                        taskParticipantsName += taskParticipants.U.Name + ", ";
                                    }
                                    if (taskParticipantsName.EndsWith(", "))
                                    {
                                        taskParticipantsName = taskParticipantsName.Remove(taskParticipantsName.Length - 2);
                                    }

                                    if (taskParticipantsName != string.Empty)
                                    {
                                        builder.Append("<tr><td style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; font-weight:normal; text-decoration:underline; padding-right:7px;padding-bottom:10px;\"> " + taskParticipantsName + " </td></tr>");
                                    }

                                    builder.Append("<tr>");

                                    builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\">" + task.Remark + "</td>");
                                    builder.Append("</tr>");
                                    builder.Append("<tr>");
                                    builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"><strong> Target Date: " + task.TargetDate.ToFormatDateString("MMM d, yyyy") + " </strong></td>");
                                    builder.Append("</tr>");

                                    if (comment != null && comment != string.Empty)
                                    {
                                        builder.Append("<tr>");
                                        builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"><strong> Comment:</strong> " + comment + "</td>");
                                        builder.Append("</tr>");
                                    }

                                    if (task.MomMeetingTaskDocuments.Count > 0)
                                    {
                                        builder.Append("<tr>");
                                        builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\"><strong>Please check below files from attached documents:</strong></td>");
                                        builder.Append("</tr>");
                                        for (int i = 1; i <= task.MomMeetingTaskDocuments.Count; i++)
                                        {
                                            MomMeetingTaskDocument itemTaskDoc = (MomMeetingTaskDocument)task.MomMeetingTaskDocuments.ElementAt(i - 1);
                                            string filePathForActionDoc = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", itemTaskDoc.DocumentPath);
                                            if (System.IO.File.Exists(filePathForActionDoc))
                                            {
                                                builder.Append("<tr>");
                                                builder.Append("<td valign=\"top\" style=\"font-family:'Trebuchet MS', Arial, Helvetica, sans-serif; font-size:14px; padding-bottom:14px;\">" + i + ") " + itemTaskDoc.DocumentPath + "</td>");
                                                builder.Append("</tr>");
                                            }
                                        }
                                    }


                                    builder.Append("</table></td>");

                                    builder.Append("</tr>");
                                    builder.Append("</table></td>");
                                    builder.Append("</tr>");
                                }

                            }

                            StringBuilder participantsbuilder = new StringBuilder();
                            string partcipantsName = string.Empty;
                            if (momMeeting.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                            {
                                if (momMeeting.MomMeetingParticipant.Count > 0)
                                {
                                    foreach (var item in momMeeting.MomMeetingParticipant)
                                    {
                                        partcipantsName += item.U.Name + ", ";
                                    }
                                }
                            }
                            //else
                            //{
                            //    if (momMeeting.MomMeetingDepartment.Count > 0)
                            //    {
                            //        foreach (var item in momMeeting.MomMeetingDepartment)
                            //        {
                            //            partcipantsName += item.Department.Name + ", ";
                            //        }
                            //    }
                            //}

                            if (partcipantsName.EndsWith(", "))
                            {
                                partcipantsName = partcipantsName.Remove(partcipantsName.Length - 2);
                            }

                            if (partcipantsName != string.Empty)
                            {
                                if (model.ParticipantType == Enums.MomMeetingParticipantType.Individual)
                                {
                                    participantsbuilder.Append("Attendees :");
                                }
                                //else
                                //{
                                //    participantsbuilder.Append("Department :");
                                //}
                            }

                            objSendMail.ValueArray = new string[] {
                                                           momMeeting.MeetingTitle != null ? "<b>" + momMeeting.MeetingTitle +"</b>" : "",
                                                            momMeeting.DateOfMeeting.ToFormatDateString("MMM d, yyyy"),
                                                            momMeeting.MeetingTime.ToString(),
                                                            momMeeting.Agenda != null ? momMeeting.Agenda : "No Description",
                                                            momMeeting.VenueName != null ? momMeeting.VenueName : "",
                                                            participantsbuilder.ToString(),
                                                            partcipantsName.ToString(),
                                                            MomMeetingTask.Count() > 0 ? builder.ToString() : "",
                                                            CurrentUser.Name
                                                        };
                            objSendMail.MailBody = objSendMail.GetHtml("MeetingEmail.html");
                            objSendMail.Subject = (momMeeting.MeetingTitle != null ? "MOM : " + momMeeting.MeetingMaster.Title + " - " + momMeeting.MeetingTitle : "null ");

                            objSendMail.To = taskParticipant.U.EmailOffice;
                            objSendMail.MailBodyManualSupply = true;


                            List<string> MomDocuments = new List<string>();
                            //foreach (var item in momMeeting.Momdocuments)
                            //{
                            //    string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", item.DocumentPath);
                            //    if (System.IO.File.Exists(FilePath))
                            //    {
                            //        MomDocuments.Add(FilePath);
                            //    }
                            //}

                            foreach (var itemMomTask in momMeeting.MomMeetingTasks)
                            {
                                foreach (var itemTaskDoc in itemMomTask.MomMeetingTaskDocuments)
                                {
                                    string filePathForActionDoc = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/Upload/MomDocument/", itemTaskDoc.DocumentPath);
                                    if (System.IO.File.Exists(filePathForActionDoc))
                                    {
                                        MomDocuments.Add(filePathForActionDoc);
                                    }
                                }
                            }

                            if (MomDocuments.Count > 0)
                            {
                                objSendMail.AttachFile = MomDocuments.ToArray();
                            }
                            objSendMail.Send();
                        }

                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult DownloadPDF(int masterId, int? id)
        {
            MomMeetingDto model = new MomMeetingDto();
            try
            {
                if (id.HasValue && id.Value > 0)
                {
                    var entity = MinutesOfMeetingService.GetMinutesOfMeetingFindById(id.Value);

                    if (entity != null)
                    {
                        if (entity.AuthorByUID == CurrentUser.Uid || entity.ChairedByUID == CurrentUser.Uid || entity.MomMeetingParticipant.Any(m => m.Uid == CurrentUser.Uid))
                        {
                            var users = userLoginService.GetAllUsersList();
                            model.AuthorName = users.FirstOrDefault(x => x.Uid == entity.AuthorByUID).Name;
                            model.ChairedName = users.FirstOrDefault(x => x.Uid == entity.ChairedByUID).Name;

                            if (entity.ParticipantType == (byte)MomMeetingParticipantType.Individual)
                            {
                                model.StrPaticipant = string.Join(", ", users.Where(a => a.PMUid != null && entity.MomMeetingParticipant.Any(s => s.Uid == a.Uid)).Select(n => (n.Name + ' ' + (n.Pmu != null ? '(' + n.Pmu.Name + ')' : ""))));
                            }
                            else
                            {
                                var Department = DepartmentService.GetActiveDepartments();
                                model.StrPaticipant = string.Join(", ", Department.Where(x => entity.MomMeetingDepartment.Any(d => d.DepartmentId == x.DeptId)).Select(n => n.Name));
                            }

                            model.Id = entity.Id;
                            model.MeetingMasterID = entity.MeetingMasterID;
                            model.MeetingMasterTitle = MinutesOfMeetingMasterService.GetMinutesOfMeetingFindById(masterId).Title;
                            model.MeetingTitle = entity.MeetingTitle;
                            model.VenueName = entity.VenueName;
                            model.MeetingTime = entity.MeetingTime;
                            model.MeetingStartTime = entity.MeetingStartTime.ToString();
                            model.DateOfMeetings = entity.DateOfMeeting.ToFormatDateString("dd/MM/yyyy");
                            model.Agenda = entity.Agenda;
                            model.Notes = entity.Notes;
                            model.AuthorByUID = entity.AuthorByUID;
                            model.ChairedByUID = entity.ChairedByUID;
                            model.ParticipantType = (MomMeetingParticipantType)entity.ParticipantType;
                            model.MomDocuments = entity.Momdocuments.Select(s => new MomdocumentDto() { Id = s.Id, DocumentPath = s.DocumentPath }).ToList();
                            //model.MomMeetingTasks = entity.MomMeetingTasks.Select(x => new MomMeetingTaskDto
                            //{
                            //        Task = x.Task,
                            //        Status = (MomMeetingStatus)x.Status,
                            //        Remark = x.Remark,
                            //        Priority = (Priority)x.Priority,
                            //        PaticipantsList = string.Join(", ", x.MomMeetingTaskParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name).ToList()),
                            //}).ToList();
                            var discussed = entity.MomMeetingTaskTimeLines.GroupBy(x => x.MomMeetingTaskId).Select(x => x.Last()).ToList();
                            var meetingData = discussed.Select(x => new { x.MomMeetingTask.Task, x.MomMeetingTask.Status, x.MomMeetingTask.Remark, x.MomMeetingTask.Priority, x.MomMeetingTask.MomMeetingTaskParticipant, x.MomMeetingTask.CreatedDate, x.MomMeetingTask.ModifiedDate, x.Comment, x.MomMeetingTaskId }).ToList();
                            var discussedList = new List<MomMeetingTaskDto>();
                            foreach (var item in meetingData)
                            {
                                var data = new MomMeetingTaskDto();
                                data = meetingData.Select(x => new MomMeetingTaskDto
                                {
                                    Task = item.Task,
                                    Status = (MomMeetingStatus)item.Status,
                                    Remark = item.Comment != null ? Regex.Replace(item.Comment, "<(.|\\n)*?>", string.Empty) : "",
                                    Priority = (Priority)item.Priority,
                                    PaticipantsList = string.Join(", ", item.MomMeetingTaskParticipant.Select(y => userLoginService.GetUserInfoByID(y.Uid).Name).ToList()),
                                }).FirstOrDefault();
                                discussedList.Add(data);
                            }
                            model.MomMeetingTasks = discussedList;

                        }
                        else
                        {
                            return MessagePartialView("Unauthorized access");
                        }
                    }
                    else
                    {
                        entity.MeetingMasterID = id ?? 0;
                    }
                }

                return new ViewAsPdf("DownloadPDF", model) { FileName = $"{model.MeetingTitle.ToSelfURL()}.pdf" };
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }

        }

    }
}
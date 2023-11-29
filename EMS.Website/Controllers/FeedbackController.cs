using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Dto.EmployeeFeedback;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Modals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static EMS.Core.Enums;

namespace EMS.Web.Controllers
{
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService feedbackservice;
        private readonly IUserLoginService userloginservice;
        private readonly IDepartmentService departmentService;

        public FeedbackController(IFeedbackService feedbackservice, IUserLoginService userloginservice, IDepartmentService departmentService)
        {
            this.feedbackservice = feedbackservice;
            this.userloginservice = userloginservice;
            this.departmentService = departmentService;
        }
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            {
                FeedbackIndexDto feedbackIndexDto = new FeedbackIndexDto();
                feedbackIndexDto.PMList = userloginservice.GetPMUsers(true).Select(u => new SelectListItem { Value = u.Uid.ToString(), Text = u.Name }).ToList();
                feedbackIndexDto.DepartmentList = departmentService.GetActiveDepartments().OrderBy(d => d.Name).Select(d => new SelectListItem { Value = d.DeptId.ToString(), Text = d.Name }).ToList();
                feedbackIndexDto.EmpFeedbackReasonList = feedbackservice.getemployeefeedbackreason().OrderBy(r => r.Name).Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name }).ToList();

                return View(feedbackIndexDto);
            }
            else
            {
                return AccessDenied();
            }

        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, EmployeeFeedbackFilter employeeFeedbackFilter)
        {
            var pagingServices = new PagingService<EmployeeFeedback>(request.Start, request.Length);
            var expr = PredicateBuilder.True<EmployeeFeedback>();

            if (employeeFeedbackFilter.pmId.HasValue)
            {
                expr = expr.And(ef => ef.UserLogin.PMUid == employeeFeedbackFilter.pmId.Value);
            }

            if (employeeFeedbackFilter.deptId.HasValue)
            {
                expr = expr.And(ef => ef.UserLogin.Department.DeptId == employeeFeedbackFilter.deptId.Value);
            }
            if (!string.IsNullOrWhiteSpace(employeeFeedbackFilter.txtEmployee))
            {
                expr = expr.And(ef => ef.UserLogin.Name.Trim().ToLower().Contains(employeeFeedbackFilter.txtEmployee.Trim().ToLower()) ||
                ef.UserLogin.EmpCode.Trim().ToLower().Contains(employeeFeedbackFilter.txtEmployee.Trim().ToLower()));
            }
            if (employeeFeedbackFilter.reasons != null)
            {
                expr = expr.And(ef => ef.EmployeeFeedbackReasonMapping.Any(a => employeeFeedbackFilter.reasons.Contains(a.EmployeeFeedbackReasonId)));
            }
            if (employeeFeedbackFilter.isEligibleForRehire != null)
            {
                expr = expr.And(ef => ef.UserLogin.UserExitProcess.Select(x => x.IsEligibleForRehire).Contains(employeeFeedbackFilter.isEligibleForRehire));
            }
            if (employeeFeedbackFilter.isVoluntryExit != null)
            {
                expr = expr.And(ef => ef.UserLogin.UserExitProcess.Select(x => x.IsVoluntaryExit).Contains(employeeFeedbackFilter.isVoluntryExit));
            }


            DateTime? dateFrom = employeeFeedbackFilter.dateFrom.ToDateTime("dd/MM/yyyy");
            DateTime? dateTo = employeeFeedbackFilter.dateTo.ToDateTime("dd/MM/yyyy");

            if (dateFrom.HasValue && dateTo.HasValue)
            {
                expr = expr.And(ef => ef.UserLogin.RelievingDate >= dateFrom && ef.UserLogin.RelievingDate <= dateTo);
            }

            else if (!string.IsNullOrWhiteSpace(employeeFeedbackFilter.dateFrom))
            {
                expr = expr.And(ef => ef.UserLogin.RelievingDate >= dateFrom);
            }
            else if (!string.IsNullOrWhiteSpace(employeeFeedbackFilter.dateTo))
            {
                expr = expr.And(ef => ef.UserLogin.RelievingDate >= dateTo);
            }


            ContextProvider.HttpContext.Session.SetString("txt_employee", employeeFeedbackFilter.txtEmployee != null ? employeeFeedbackFilter.txtEmployee.ToString() : "");
            ContextProvider.HttpContext.Session.SetString("ddl_dept", employeeFeedbackFilter.deptId > 0 ? employeeFeedbackFilter.deptId.ToString() : "");
            ContextProvider.HttpContext.Session.SetString("ddl_pm", employeeFeedbackFilter.pmId > 0 ? employeeFeedbackFilter.pmId.ToString() : "");
            ContextProvider.HttpContext.Session.SetString("txt_dateFrom", dateFrom != null ? dateFrom.ToString() : "");
            ContextProvider.HttpContext.Session.SetString("txt_dateTo", dateTo != null ? dateTo.ToString() : "");
            ContextProvider.HttpContext.Session.SetString("txt_reasons", employeeFeedbackFilter.reasons != null ? string.Join(",", employeeFeedbackFilter.reasons).ToString() : "");
            ContextProvider.HttpContext.Session.SetString("ddl_isEligibleForRehire", employeeFeedbackFilter.isEligibleForRehire !=null ? employeeFeedbackFilter.isEligibleForRehire.ToString() : "");
            ContextProvider.HttpContext.Session.SetString("ddl_isVoluntryExit", employeeFeedbackFilter.isVoluntryExit!=null ? employeeFeedbackFilter.isVoluntryExit.ToString() : "");

            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.UserLogin.Name);

                        case "Department":
                            return o.OrderByColumn(item, c => c.UserLogin.Name);

                        default:
                            return o.OrderByColumn(item, c => c.CreatedDate);

                    }
                }
                return o.OrderByDescending(c => c.ModifyDate);
            };
            int totalCount = 0;
            var response = feedbackservice.GetfeedbackByPaging(out totalCount, pagingServices);
            var result = DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                r.Id,
                uid = r.EmpUid,
                rowIndex = (index + 1) + (request.Start),
                userName = (r.UserLogin.EmpCode == "" || r.UserLogin.EmpCode == null) ? r.UserLogin.Name : r.UserLogin.Name + " " + "( " + r.UserLogin.EmpCode + ")",
                pmName = (r.UserLogin.PMUid != null && r.UserLogin.PMUid > 0) ? '(' + r.UserLogin.Pmu.Name + ')' : "",
                department = r.UserLogin.Department.Name,
                Designation = (r.UserLogin.Role != null ? r.UserLogin.Role.RoleName : ""),
                feedBackReasons = string.Join(", ", r.EmployeeFeedbackReasonMapping.Where(e => e.EmployeeFeedbackId == r.Id)
                                    .Select(c => c.EmployeeFeedbackReason.Name).ToList()),
                JoiningDate = r.UserLogin.JoinedDate.HasValue ? ((DateTime)r.UserLogin.JoinedDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                LeavingDate = r.UserLogin.RelievingDate.HasValue ? ((DateTime)r.UserLogin.RelievingDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                //r.LeavingDate.ToString("dd/MM/yyyy"),
                r.CreatedDate,
            }));

            return result;
        }

        [HttpGet]
        //[CustomActionAuthorization]
        public ActionResult Add(int? id)
        {
            int userId = 0;
            if (CurrentUser.Uid == 0)
            {
                var returnurl = SiteKey.DomainName + "feedback/add";
                return RedirectToAction("index", "login", new { ReturnUrl = returnurl });
            }
            else
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.HRBP || CurrentUser.RoleId == (int)Enums.UserRoles.Director)
                {
                    if (CurrentUser.Uid == userId)
                    {
                        // need to implement method

                    }
                    else
                    {
                        // manage user details
                       userId = id.Value;
                    }
                }
                else
                {
                    userId = CurrentUser.Uid;
                    var getUser = userloginservice.GetUsersById(userId);
                    if (getUser != null && getUser.IsResigned != true)
                    {
                        string msg = "Sorry! only on notice user can fill this exit Interview form.";
                        return RedirectToAction("thankyou", "feedback", new { message = msg });
                    }
                }
                // check currentuser is in isResigned != true
            }

            FeedbackDto model = new FeedbackDto();
            model.IsFeedbackSubmited = feedbackservice.FeedbackExists(userId);
            List<SelectListItem> reasons = new List<SelectListItem>();
            var reason = feedbackservice.getemployeefeedbackreason();
            var rank = feedbackservice.getemployeefeedbackrank();
            List<SelectListItem> leavingreason = new List<SelectListItem>();
            leavingreason = Enum.GetValues(typeof(FeedbackRankStatus)).Cast<FeedbackRankStatus>().Select(v => new SelectListItem
            {
                Text = v.GetDescription(),
                Value = ((int)v).ToString()
            }).ToList();

            var feedback = feedbackservice.employeeFeedbackbyUid(userId);

            if (userId > 0 && feedback != null)
            {
                // var feedback = feedbackservice.findByData(id.Value);
                //var feedback = feedbackservice.employeeFeedbackbyUid(userId);
                foreach (var item in reason)
                {
                    // bool ischecked = viewdata.EmployeeFeedbackReasons.Where(s => s.Id == item.Id).Any();
                    bool ischecked = feedback.EmployeeFeedbackReasonMapping.Where(s => s.EmployeeFeedbackReasonId == item.Id).Any();

                    reasons.Add(new SelectListItem
                    { Text = item.Name, Value = item.Id.ToString(), Selected = ischecked });
                }
                foreach (var item in rank)
                {
                    var temp = feedback.EmployeeFeedbackRankStatus.FirstOrDefault(s => s.EmployeeFeedbackRankId == item.Id && s.EmployeeFeedbackId == feedback.Id)?.FeedBackStatus;
                    model.FeedbackrankDto.Add(new FeedbackrankDto { Name = item.Name, EmployeeFeedbackRankId = item.Id, EmployeeFeedbackStatus = temp });
                }
                model.Name = feedback.UserLogin.Name;
                model.EmpCode = feedback.UserLogin.EmpCode;
                model.EmailOffice = feedback.UserLogin.EmailOffice;
                model.Designation = feedback.UserLogin.Role.RoleName;
                model.Suggestions = feedback.Suggestion;
                model.ReviewLink = feedback.ReviewLink;
                model.LFProfile = feedback.Lfprofile;
                model.EmailSkypePassReset = feedback.EmailSkypePassReset;
                model.comment = feedback.Comment;
                model.LeavingDate = feedback.LeavingDate.HasValue ? ((DateTime)feedback.LeavingDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                //feedback.LeavingDate.ToString("dd/MM/yyyy");
                //model.Id = id.Value;
                model.Id = feedback.Id;
                model.Department = feedback.UserLogin?.Department?.Name;
                model.PMName = feedback.UserLogin.PMUid.HasValue && feedback.UserLogin.PMUid > 0 ? userloginservice.GetUsersById((int)feedback.UserLogin.PMUid).Name : "";
            }
            else
            {
                var userinfo = userloginservice.GetUserInfoByID(CurrentUser.Uid);
                foreach (var item in reason)
                {
                    reasons.Add(new SelectListItem
                    { Text = item.Name, Value = item.Id.ToString() });
                }
                foreach (var item in rank)
                {
                    model.FeedbackrankDto.Add(new FeedbackrankDto { Name = item.Name, EmployeeFeedbackRankId = item.Id });
                }
                model.Userinfo = userinfo;
                model.Name = userinfo.Name;
                model.EmailOffice = userinfo.EmailOffice;
                model.EmpCode = userinfo.EmpCode;
                model.Designation = userinfo.JobTitle;
                model.Department = userinfo.Department?.Name;
                model.PMName = userinfo.PMUid.HasValue && userinfo.PMUid > 0 ?
                    userloginservice.GetUsersById((int)userinfo.PMUid).Name : "";

            }
            // set EmployeeFeedback data
            var empFeedbackdata = feedbackservice.employeeFeedbackbyUid(userId);
            model.LFProfile = empFeedbackdata != null ? empFeedbackdata.Lfprofile : false;
            model.EmailSkypePassReset = empFeedbackdata != null ? empFeedbackdata.EmailSkypePassReset : false;
            model.Id = empFeedbackdata != null ? empFeedbackdata.Id : 0;

            model.Feedbackreason = reasons;
            model.LeavingReason = leavingreason;
            return View(model);
        }
        [HttpPost]
        //[CustomActionAuthorization]
        public ActionResult Add(FeedbackDto model)
        {
            if (CurrentUser.Uid == 0)
            {
                var returnurl = SiteKey.DomainName + "feedback/add";
                return RedirectToAction("index", "login", new { ReturnUrl = returnurl });
            }

            if (feedbackservice.FeedbackExists(model.Uid))
            {
                return MessagePartialView("Feedback already exists", "Already exists", MessageType.Danger);
            }

            model.SaveBy = CurrentUser.Uid;
            feedbackservice.Save(model);
            feedbackservice.updateFeedbackStatus(model.Uid);

            //ShowSuccessMessage("Success!", "Thank you for completing this information. Your responses will be treated with total confidentiality.", false);
            //return NewtonSoftJsonResult(new RequestOutcome<string> { RedirectUrl = Url.Action("index", "feedback"), IsSuccess = true });
            return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = true, RedirectUrl = Url.Action("thankyou", "feedback") });

        }

        [HttpGet]
        public IActionResult ThankYou(string message)
        {
            ViewBag.message = string.IsNullOrEmpty(message) != true ? message : " Thank you for submitting your feedback.";
            return View();
        }

        [HttpPost]
        public IActionResult GetFeedbackReasonResult(EmployeeFeedbackFilter employeeFeedbackFilter)
        {
            var result = (dynamic)null;
            try
            {
                //DateTime? startDate = (String.IsNullOrEmpty(employeeFeedbackFilter.dateFrom) ?
                //                   (DateTime?)null : DateTime.Parse(employeeFeedbackFilter.dateFrom));

                //var dateTo = employeeFeedbackFilter.dateTo;
                //if (!string.IsNullOrEmpty(dateTo))
                //{

                //    var firstPartDate = dateTo.Split("/")[0];
                //    var secondPartDate = dateTo.Split("/")[1];
                //    var thirdPartDate = dateTo.Split("/")[2];

                //    if (Convert.ToInt16(firstPartDate) > 12)
                //    {
                //        dateTo = secondPartDate + "/" + firstPartDate + "/" + thirdPartDate;
                //    }
                //}

                //DateTime? endDate = (String.IsNullOrEmpty(dateTo) ?
                //                         (DateTime?)null : DateTime.Parse(dateTo));

                DateTime? startDate = !string.IsNullOrWhiteSpace(employeeFeedbackFilter.dateFrom) ? Convert.ToDateTime(employeeFeedbackFilter.dateFrom) : (DateTime?)null;
                DateTime? endDate = !string.IsNullOrWhiteSpace(employeeFeedbackFilter.dateTo) ? Convert.ToDateTime(employeeFeedbackFilter.dateTo) : (DateTime?)null;

                string empNameCode = Convert.ToString(employeeFeedbackFilter.txtEmployee);
                //var mappingList = feedbackservice
                result = feedbackservice.getemployeefeedbackreason().Where(a => employeeFeedbackFilter.reasons != null ? employeeFeedbackFilter.reasons.Contains(a.Id) : true).Select(
                   r => new
                   {
                       //EmployeeFeedbackId = r.Id,
                       Feedbackname = r.Name,
                       Color = r.Color ?? "Orange",
                       totalFeedBack = r.EmployeeFeedbackReasonMapping
                                       .Where(c => c.EmployeeFeedbackReasonId == r.Id
                                        && (employeeFeedbackFilter.pmId == null ? true :
                                        c.EmployeeFeedback.UserLogin.PMUid == employeeFeedbackFilter.pmId) // filter according to  pmUid 
                                        && (employeeFeedbackFilter.deptId == null ? true :
                                        c.EmployeeFeedback.UserLogin.DeptId == employeeFeedbackFilter.deptId) // filter according to deptId
                                        && (empNameCode == null ? true :
                                        ((c.EmployeeFeedback.UserLogin.EmpCode != null && c.EmployeeFeedback.UserLogin.EmpCode.ToLower() == empNameCode.ToLower()) // filter according to empcode
                                        || (c.EmployeeFeedback.UserLogin.Name != null && c.EmployeeFeedback.UserLogin.Name.ToLower() == empNameCode.ToLower()))) // filter according to Name
                                        && (startDate == null && endDate == null ? true :
                                        (c.EmployeeFeedback.UserLogin.RelievingDate != null && // filter nullable joining date record
                                        c.EmployeeFeedback.UserLogin.RelievingDate >= startDate && c.EmployeeFeedback.UserLogin.RelievingDate <= endDate))
                                        ).Count(),

                   }).ToList().OrderBy(c => c.totalFeedBack);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;

            }
            return Json(result);
        }

        [HttpPost]
        public IActionResult GetLeaveReasonByPM(EmployeeFeedbackFilter employeeFeedbackFilter)
        {
            var result = (dynamic)null;
            try
            {

                DateTime? startDate = employeeFeedbackFilter.dateFrom.ToDateTime("dd/MM/yyyy");
                DateTime? endDate = employeeFeedbackFilter.dateTo.ToDateTime("dd/MM/yyyy");

                //var dateFrom = employeeFeedbackFilter.dateFrom;
                //if (!string.IsNullOrEmpty(dateFrom))
                //{

                //    var firstPartDate = dateFrom.Split("/")[0];
                //    var secondPartDate = dateFrom.Split("/")[1];
                //    var thirdPartDate = dateFrom.Split("/")[2];

                //    if (Convert.ToInt16(firstPartDate) > 12)
                //    {
                //        dateFrom = secondPartDate + "/" + firstPartDate + "/" + thirdPartDate;
                //    }
                //}

                //DateTime? startDate = (string.IsNullOrEmpty(dateFrom) ?
                //                   (DateTime?)null : DateTime.Parse(dateFrom));

                //var dateTo = employeeFeedbackFilter.dateTo;
                //if (!string.IsNullOrEmpty(dateTo))
                //{

                //    var firstPartDate = dateTo.Split("/")[0];
                //    var secondPartDate = dateTo.Split("/")[1];
                //    var thirdPartDate = dateTo.Split("/")[2];

                //    if (Convert.ToInt16(firstPartDate) > 12)
                //    {
                //        dateTo = secondPartDate + "/" + firstPartDate + "/" + thirdPartDate;
                //    }
                //}

                //DateTime? endDate = (string.IsNullOrEmpty(dateTo) ?
                //                         (DateTime?)null : DateTime.Parse(dateTo));

                string empNameCode = Convert.ToString(employeeFeedbackFilter.txtEmployee);

                var empFeedback = feedbackservice.employeefeedback().ToList();
                var pmData = userloginservice.GetPMUsers().Where(p => p.RoleId == (int)Core.Enums.UserRoles.PM &&
                             (employeeFeedbackFilter.pmId == null ? true : p.Uid == employeeFeedbackFilter.pmId)).ToList();
                result = pmData.Select((r, index) => new
                {
                    count = index + 1,
                    pmName = r.Name,
                    color = GetColor().Where(c => c.Value == Convert.ToString(index + 1)).Select(a => a.Text).FirstOrDefault() ?? "Orange",
                    totalFeedBack =
                    empFeedback.Where(e => e.UserLogin.PMUid == r.Uid &&
                                          (empNameCode == null ? true :
                                         ((e.UserLogin.EmpCode != null && e.UserLogin.EmpCode.ToLower() == empNameCode.ToLower())
                                          || (e.UserLogin.Name != null && e.UserLogin.Name.ToLower() == empNameCode.ToLower())
                                          )) && (employeeFeedbackFilter.reasons != null ?
                                            e.EmployeeFeedbackReasonMapping.Any(a => employeeFeedbackFilter.reasons.Contains(a.EmployeeFeedbackReasonId)) : true)
                                            && (employeeFeedbackFilter.deptId == null ?
                                           true : e.UserLogin.DeptId == employeeFeedbackFilter.deptId) &&
                                             (startDate == null && endDate == null ? true :
                                            (e.UserLogin.RelievingDate != null &&
                                            e.UserLogin.RelievingDate >= startDate && e.UserLogin.RelievingDate <= endDate))
                                          )
                                          .Count()

                }).ToList().OrderBy(c => c.totalFeedBack);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;

            }
            return Json(result);
        }

        [HttpGet]
        private List<SelectListItem> GetColor()
        {
            var colorList = new List<KeyValuePair<string, int>>();
            colorList.Add(new KeyValuePair<string, int>("Violet", 1));
            colorList.Add(new KeyValuePair<string, int>("Green", 2));
            colorList.Add(new KeyValuePair<string, int>("Pink", 3));
            colorList.Add(new KeyValuePair<string, int>("Orange", 4));
            colorList.Add(new KeyValuePair<string, int>("Blue", 5));
            colorList.Add(new KeyValuePair<string, int>("Brown", 6));
            colorList.Add(new KeyValuePair<string, int>("Red", 7));
            colorList.Add(new KeyValuePair<string, int>("Purple", 8));
            colorList.Add(new KeyValuePair<string, int>("Indigo", 9));
            colorList.Add(new KeyValuePair<string, int>("CadetBlue", 10));
            colorList.Add(new KeyValuePair<string, int>("DarkSlateBlue", 11));
            colorList.Add(new KeyValuePair<string, int>("Grey", 12));
            colorList.Add(new KeyValuePair<string, int>("Darkpink", 13));
            colorList.Add(new KeyValuePair<string, int>("Yellow", 14));
            colorList.Add(new KeyValuePair<string, int>("Gold", 15));
            colorList.Add(new KeyValuePair<string, int>("Chocolate", 16));
            colorList.Add(new KeyValuePair<string, int>("DarkSlateGrey", 17));

            var returnTypeList = new List<CommonListType>();
            var list = colorList.Select(x => new SelectListItem
            {
                Text = x.Key.ToString(),
                Value = x.Value.ToString(),
            }).ToList();

            return list;
        }

        [HttpGet]
        public string GetPMName(int PmUid)
        {
            var pmName = string.Empty;
            var userdata = userloginservice.GetUserInfoByID(PmUid);
            if (userdata != null)
            {
                pmName = userdata.Name;
            }
            return pmName;
        }


        public ActionResult ExportFeedbackDataReportToExcel(IDataTablesRequest request)//, int? pmid
        {
            //var pagingServices = new PagingService<EmployeeFeedback>(request.Start, request.Length);
            var expr = PredicateBuilder.True<EmployeeFeedback>();

            //var pagingService1 = new PagingService<UserLogin>(request.Start, request.Length);
            PagingService<EmployeeFeedback> pagingServices = new PagingService<EmployeeFeedback>(request == null ? 0 : request.Start, request == null ? 0 : request.Length);

            string empNameCode = HttpContext.Session.GetString("txt_employee");
            string pmId = HttpContext.Session.GetString("ddl_pm");
            string deptId = HttpContext.Session.GetString("ddl_dept");
            string datefrom = HttpContext.Session.GetString("txt_dateFrom");
            string dateTo = HttpContext.Session.GetString("txt_dateTo");
            string reasons = HttpContext.Session.GetString("txt_reasons");
            string isVoluntryExit = HttpContext.Session.GetString("ddl_isVoluntryExit");
            string isEligibleForRehire = HttpContext.Session.GetString("ddl_isEligibleForRehire");

            //DateTime? frmDate = !string.IsNullOrWhiteSpace(datefrom) ?
            //        DateTime.ParseExact(datefrom, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;
            //DateTime? tDate = !string.IsNullOrWhiteSpace(dateTo) ?
            //    DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture) : (DateTime?)null;

            DateTime? frmDate = !string.IsNullOrWhiteSpace(datefrom) ? Convert.ToDateTime(datefrom) : (DateTime?)null;
            DateTime? tDate = !string.IsNullOrWhiteSpace(dateTo) ? Convert.ToDateTime(dateTo) : (DateTime?)null;

            if (!string.IsNullOrEmpty(pmId))
            {
                expr = expr.And(e => e.UserLogin.PMUid == Convert.ToInt32(pmId));
            }
            if (!string.IsNullOrEmpty(deptId))
            {
                expr = expr.And(e => e.UserLogin.DeptId == Convert.ToInt32(deptId));
            }
            if (!string.IsNullOrWhiteSpace(empNameCode))
            {
                expr = expr.And(ef => ef.UserLogin.Name.Trim().ToLower().Contains(empNameCode.Trim().ToLower()) ||
                ef.UserLogin.EmpCode.Trim().ToLower().Contains(empNameCode.Trim().ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(reasons))
            {
                int[] data = Array.ConvertAll<string, int>(reasons.Split(','), Convert.ToInt32);
                expr = expr.And(ef => ef.EmployeeFeedbackReasonMapping.Any(a => data.Contains(a.EmployeeFeedbackReasonId)));
            }
            if (!string.IsNullOrWhiteSpace(datefrom) && !string.IsNullOrWhiteSpace(dateTo))
            {
                expr = expr.And(ef => ef.UserLogin.RelievingDate >= frmDate && ef.UserLogin.RelievingDate <= tDate);
            }
            else if (!string.IsNullOrWhiteSpace(datefrom) && string.IsNullOrWhiteSpace(dateTo))
            {
                expr = expr.And(ef => ef.UserLogin.RelievingDate >= frmDate);
            }
            else if (!string.IsNullOrWhiteSpace(dateTo) && string.IsNullOrWhiteSpace(datefrom))
            {
                expr = expr.And(ef => ef.UserLogin.RelievingDate <= tDate);
            }
            if (!string.IsNullOrEmpty(isEligibleForRehire))
            {
                expr = expr.And(ef => ef.UserLogin.UserExitProcess.Select(x => x.IsEligibleForRehire).Contains(Convert.ToBoolean(isEligibleForRehire)));
            }
            if (!string.IsNullOrEmpty(isVoluntryExit))
            {
                expr = expr.And(ef => ef.UserLogin.UserExitProcess.Select(x => x.IsEligibleForRehire).Contains(Convert.ToBoolean(isVoluntryExit)));
            }


            
            //if (frmDate != null && tDate != null)
            //{
            //    expr = expr.And(e => e.UserLogin.JoinedDate.Value.Day >= frmDate.Value.Day && e.UserLogin.JoinedDate.Value.Month >= frmDate.Value.Month &&
            //    e.UserLogin.JoinedDate.Value.Day <= tDate.Value.Day && e.UserLogin.JoinedDate.Value.Month <= tDate.Value.Month);
            //}
            //else if (frmDate != null)
            //{
            //    expr = expr.And(e => e.UserLogin.JoinedDate.Value.Day == frmDate.Value.Day && e.UserLogin.JoinedDate.Value.Month == frmDate.Value.Month);
            //}
            //else if (tDate != null)
            //{
            //    expr = expr.And(e => e.UserLogin.JoinedDate.Value.Day == tDate.Value.Day && e.UserLogin.JoinedDate.Value.Month == tDate.Value.Month);
            //}

            pagingServices.Filter = expr;

            //pagingService.Sort = (o) =>
            //{
            //    return o.Where(c => c.IsActive == true).OrderBy(a => a.DOB.Value.Month).ThenBy(x => x.DOB.Value.Day);
            //};

            int totalCount = 0;
            var response = feedbackservice.GetfeedbackByPaging(out totalCount, pagingServices);

            List<EmployeeFeedbackReportDto> responseReport = response.Select(r => new EmployeeFeedbackReportDto()
            {
                EmpNameCode = r.UserLogin.Name + " " + (r.UserLogin.EmpCode == null ? " " : "(" + r.UserLogin.EmpCode + ")"),
                ProjectManager = (r.UserLogin.PMUid != null && r.UserLogin.PMUid > 0) ? '(' + r.UserLogin.Pmu.Name + ')' : "",
                //r.UserLogin.PMUid > 0 ? (GetPMName(Convert.ToInt32(r.UserLogin.PMUid))) : "",
                Designation = (r.UserLogin.Role != null ? r.UserLogin.Role.RoleName : ""),
                Department = r.UserLogin.Department.Name == null ? "" : r.UserLogin.Department.Name,
                FeedbackReasons = string.Join(", ", r.EmployeeFeedbackReasonMapping.Where(e => e.EmployeeFeedbackId == r.Id)
                                    .Select(c => c.EmployeeFeedbackReason.Name).ToList()),
                JoiningDate = r.UserLogin.JoinedDate.HasValue ? ((DateTime)r.UserLogin.JoinedDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                LeavingDate = r.UserLogin.RelievingDate.HasValue ? ((DateTime)r.UserLogin.RelievingDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "",
                //r.LeavingDate.ToString("dd/MM/yyyy"),
                CreatedDate = r.CreatedDate.ToString("dd/MM/yyyy")
            }).ToList();


            string Reportname = "Employee-Feedback-Report";
            int subsheet = 0;
            List<ExportExcelColumn> excelColumn = new List<ExportExcelColumn>();

            excelColumn.Add(new ExportExcelColumn { ColumnName = "Sr. No.", PropertyName = "SrNo", ColumnWidth = 2500 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "NAME", PropertyName = "Name", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "PM", PropertyName = "PM", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "DESIGNATION", PropertyName = "DESIGNATION", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "DEPARTMENT", PropertyName = "DEPARTMENT", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "FEEDBACKREASONS", PropertyName = "FEEDBACKREASONS", ColumnWidth = 50000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "JOININGDATE", PropertyName = "JOININGDATE", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "LEAVINGDATE", PropertyName = "LEAVINGDATE", ColumnWidth = 3500 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "CREATEDDATE", PropertyName = "CREATEDDATE", ColumnWidth = 3500 });


            var memoryStream = ToExportToExcel(responseReport, subsheet, Reportname, excelColumn);

            return File(memoryStream.ToArray(), "application/vnd.ms-excel", Reportname.Trim().Replace(" ", "_") + "_" + DateTime.Now.ToString("hh_mm_ss_tt") + ".xls");
        }

        public static MemoryStream ToExportToExcel(List<EmployeeFeedbackReportDto> lsObj, int isSubsheet, string Reportname, List<ExportExcelColumn> excelColumn)
        {
            MemoryStream response = new MemoryStream();
            if (lsObj != null && lsObj.Count() > 0)
            {
                bool columnFlag = false;
                List<string> props = new List<string>();
                List<string> childprops = new List<string>();
                //Get the column names of Employee Birthday Data
                if (excelColumn != null && excelColumn.Count > 0)
                {
                    columnFlag = true;
                    props = excelColumn.Select(s => s.PropertyName.Trim()).ToList();
                }

                //Get the column names of Employee Birthday Data        
                if (props != null && props.Count > 0)
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

                    var sheet = workbook.CreateSheet(Reportname);
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

                    //Create a header row
                    var headerRow = sheet.CreateRow(0);
                    //Set the column names in the header row

                    var count = 0;
                    foreach (var item in props)
                    {
                        headerRow.CreateCell(count, CellType.STRING).SetCellValue(
                         columnFlag ? excelColumn.Where(x => x.PropertyName == item).Select(x => x.ColumnName).FirstOrDefault() : item);
                        count = count + 1;
                    }

                    for (int i = 0; i < headerRow.LastCellNum; i++)
                    {
                        headerRow.GetCell(i).CellStyle = attendeeLabelCellStyle;
                        headerRow.Height = 350;
                    }

                    //(Optional) freeze the header row so it is not scrolled
                    sheet.CreateFreezePane(0, 1, 0, 1);

                    int rowNumber = 1; // Index of Row for data

                    var rowCellStyleattendeeothercolumn = workbook.CreateCellStyle();
                    rowCellStyleattendeeothercolumn.VerticalAlignment = VerticalAlignment.CENTER;

                    var otherattendeeLabelFont = workbook.CreateFont();
                    otherattendeeLabelFont.FontHeightInPoints = 9;
                    rowCellStyleattendeeothercolumn.SetFont(otherattendeeLabelFont);

                    for (int i = 0; i < lsObj.Count(); i++)
                    {
                        var row = sheet.CreateRow(rowNumber);

                        count = 0;
                        row.CreateCell(count++).SetCellValue(i + 1); // Sr. No.
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].EmpNameCode));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].ProjectManager));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Designation));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Department));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].FeedbackReasons));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].JoiningDate));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].LeavingDate));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].CreatedDate));

                        rowNumber++;

                        for (int k = 0; k < headerRow.LastCellNum; k++)
                        {
                            string columnName = headerRow.GetCell(k).ToString();
                            if (k == 0)
                            {
                                sheet.SetColumnWidth(k, 2500);
                            }
                            else if (k == 5)
                            {
                                sheet.SetColumnWidth(k, 50000);
                            }
                            else
                            {
                                sheet.SetColumnWidth(k, 5500);
                            }
                        }
                        row.Height = 350;
                    }
                    workbook.Write(response);
                    //Return the result to the end user
                }
            }
            return response;
        }

        [HttpGet]
        public ActionResult EmpFeedback()
        {
            if (CurrentUser.RoleId == (int)Enums.UserRoles.Director || RoleValidator.HR_RoleIds.Contains(CurrentUser.RoleId))
            {
                EmpFeedbackDto model = new EmpFeedbackDto();
                var reason = feedbackservice.getemployeefeedbackreason();
                var rank = feedbackservice.getemployeefeedbackrank();
                List<SelectListItem> reasons = new List<SelectListItem>();
                List<SelectListItem> leavingreason = new List<SelectListItem>();
                List<SelectListItem> pmusers = new List<SelectListItem>();
                List<SelectListItem> feedbankRank = new List<SelectListItem>();
                var pmUsers = userloginservice.GetPMUsers();

                leavingreason = Enum.GetValues(typeof(FeedbackRankStatus)).Cast<FeedbackRankStatus>().Select(v => new SelectListItem
                {
                    Text = v.GetDescription(),
                    Value = ((int)v).ToString()
                }).ToList();

                foreach (var item in reason)
                {
                    reasons.Add(new SelectListItem
                    { Text = item.Name, Value = item.Id.ToString() });
                }

                List<FeedbackrankDto> lstFeedbackrankDto = new List<FeedbackrankDto>();

                foreach (var item in rank)
                {
                    FeedbackrankDto newItem = new FeedbackrankDto();
                    newItem.Name = item.Name;
                    newItem.EmployeeFeedbackRankId = item.Id;
                    lstFeedbackrankDto.Add(newItem);
                }

                model.FeedbackrankDto = lstFeedbackrankDto;

                foreach (var item in pmUsers)
                {
                    pmusers.Add(new SelectListItem { Text = item.Name, Value = item.Uid.ToString() });
                }

                model.Feedbackreason = reasons;
                model.LeavingReason = leavingreason;
                model.PMUsers = pmusers;

                return View(model);
            }
            else
            {
                return AccessDenied();
            }
        }

        [HttpPost]
        public ActionResult GetEmployeesByPM(int pmid)
        {
            //var users = (pmid > 0 ? userloginservice.GetUsersByPM(pmid).Where(a=>a.IsResigned == true).ToList() : userloginservice.GetUsers(true)).Where(a =>a.IsResigned == true).Select(u => new { text = u.EmpCode != null ? u.Name.ToString() + " [" + u.EmpCode + "]" : u.Name, value = u.Uid });
            var users = (pmid > 0 ? userloginservice.GetResignedUsersByPM(pmid).Where(a => a.IsResigned == true).ToList() : userloginservice.GetUsers(true)).Where(a => a.IsResigned == true).Select(u => new { text = u.EmpCode != null ? u.Name.ToString() + " [" + u.EmpCode + "]" : u.Name, value = u.Uid });

            return Json(users);
        }

        [HttpPost]
        public ActionResult GetEmployeesDataByid(int empid)
        {
            EmpInfoDto empInfo = new EmpInfoDto();

            empInfo.IsfeedSubmitted = false;

            var userInfo = userloginservice.GetUserInfoByID(empid);
            if (userInfo != null)
            {
                empInfo.Uid = userInfo.Uid;
                empInfo.Name = userInfo.Name.ToTitleCase() ?? "";
                empInfo.EmailOffice = userInfo.EmailOffice ?? "";
                empInfo.EmpCode = userInfo.EmpCode ?? "";
                empInfo.Department = userInfo.Department.Name;
                empInfo.Designation = userInfo.Role != null ? userInfo.Role.RoleName : "";
                empInfo.PMName = (userInfo.PMUid != null && userInfo.PMUid > 0) ? '(' + userInfo.Pmu.Name + ')' : ""; 

                bool checkfeedback = feedbackservice.FeedbackExists(empInfo.Uid);

                if (checkfeedback == true)
                {
                    empInfo.IsfeedSubmitted = true;
                }
            }
            else
            {
                empInfo.Name = empInfo.EmailOffice = empInfo.EmpCode = empInfo.Department = empInfo.Designation = empInfo.PMName = "";
            }

            return Json(empInfo);
        }

        [HttpPost]
        public ActionResult savempfeedback(EmpFeedbackDto model)
        {
            try
            {
                if (feedbackservice.FeedbackExists(model.Uid))
                {
                    ShowErrorMessage("Error", "Failed..!! Feedback already exists", false);
                }
                else
                {
                    model.SaveBy = CurrentUser.Uid;
                    feedbackservice.Savefeedback(model);
                    ShowSuccessMessage("Success", "User Feedback details successfully Saved", false);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }
            return RedirectToAction("index");
        }
    }
}
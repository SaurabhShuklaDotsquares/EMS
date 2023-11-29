using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class InvestmentController : BaseController
    {
        #region Fields and Constructor

        private readonly IInvestmentService investmentService;
        private readonly IUserLoginService userLoginService;
        private readonly IActionContextAccessor actionContextAccessor;

        public InvestmentController(IInvestmentService _investmentService, IUserLoginService _userloginservice, IActionContextAccessor _actionContextAccessor)
        {
            this.investmentService = _investmentService;
            this.userLoginService = _userloginservice;
            this.actionContextAccessor = _actionContextAccessor;
        }

        #endregion

        [HttpGet]
        public ActionResult Index()
        {
            var model = new InvestmentIndexDto
            {
                FinancialYearList = investmentService.GetFinancialYears().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList()
            };

            if (CurrentUser.DeptId == (int)Enums.ProjectDepartment.AccountDepartment)
            {
                model.PMUserList = userLoginService.GetUserByRole((int)Enums.UserRoles.PM, true).Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() }).ToList();
            }

            var currentFY = getCurrentFinancialYear();
            var investment = investmentService.GetinvestmentByUserAndFY(CurrentUser.Uid, currentFY.Item1);
            model.ShowAddNewOption = investment == null;

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(IDataTablesRequest request, int? financialYear, int? pmId, string textSearch)
        {
            var pagingServices = new PagingService<Investment>(request.Start, request.Length);
            var filterExpr = PredicateBuilder.True<Investment>();
            var currentUserId = CurrentUser.Uid;

            if (CurrentUser.DeptId == (int)Enums.ProjectDepartment.AccountDepartment)
            {
                filterExpr = filterExpr.And(x => !x.IsDraft || x.UserloginId == currentUserId);
            }
            else
            {
                filterExpr = filterExpr.And(x => x.UserloginId == currentUserId);
            }

            if (financialYear.HasValue && financialYear.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.FinancialYearId == financialYear.Value);
            }

            if (pmId.HasValue && pmId.Value > 0)
            {
                filterExpr = filterExpr.And(x => x.UserloginId == pmId.Value || x.UserLogin.PMUid == pmId.Value);
            }

            if (!string.IsNullOrWhiteSpace(textSearch))
            {
                textSearch = textSearch.Trim().ToLower();
                filterExpr = filterExpr.And(x => x.UserLogin.Name.ToLower().Contains(textSearch) || x.AttendanceCode.ToLower().Contains(textSearch));
            }

            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "Name":
                            return o.OrderByColumn(item, c => c.Name);

                        default:
                            return o.OrderByColumn(item, c => c.CreateDate);

                    }
                }
                return o.OrderByDescending(c => c.ModifyDate);
            };

            int totalCount = 0;
            var response = investmentService.GetinvestmentByPaging(out totalCount, pagingServices);

            return DataTablesJsonResult(totalCount, request, response.Select((r, index) => new
            {
                Id = r.Id.ToUrlBase64String(),
                rowIndex = (index + 1) + (request.Start),
                Name = r.Name,
                FatherName = r.FatherName,
                AttendanceCode = r.AttendanceCode,
                FinancialYear = r.FinancialYear.Name,
                CreateDate = r.CreateDate.ToFormatDateString("dd MMM yyyy, hh:mm tt"),
                ModifyDate = r.ModifyDate.ToFormatDateString("dd MMM yyyy, hh:mm tt"),
                AllowEdit = (r.UserloginId == currentUserId && r.IsDraft),
                DownloadPDF = !r.IsDraft,
                DownloadZip = !r.IsDraft && r.InvestmentDocuments.Any()
            }));
        }

        [HttpGet]
        public ActionResult AddEdit(string id)
        {
            try
            {
                InvestmentDto model = new InvestmentDto();

                var currentFY = getCurrentFinancialYear();
                var startDate = currentFY.Item2;
                var endDate = currentFY.Item3;

                string currentFinYear = string.Format("{0}-{1}", startDate.Year, endDate.Year);

                Investment investment = null;

                int? invId = !string.IsNullOrWhiteSpace(id) ? id.UrlBaseToInt32() : (int?)null;

                if (invId.HasValue && invId.Value > 0)
                {
                    investment = investmentService.GetinvestmentById(invId.Value);

                    if (investment == null || investment.UserloginId != CurrentUser.Uid)
                    {
                        return CustomErrorView("Unable to get investment profile");
                    }
                    else if (!investment.IsDraft || currentFinYear != string.Format("{0}-{1}", investment.FinancialYear.FromYear, investment.FinancialYear.ToYear))
                    {
                        return RedirectToAction("DownloadPdf", new { id = investment.Id.ToUrlBase64String() });
                    }
                    else
                    {
                        model.Id = investment.Id;
                        model.Name = investment.Name;
                        model.FatherName = investment.FatherName;
                        model.AttendanceCode = investment.AttendanceCode;
                        model.PAN = investment.PAN;
                        model.HomeAddress = investment.HomeAddress;
                        model.DOB = investment.DOB.ToFormatDateString("dd/MM/yyyy");
                        model.FinancialYear = investment.FinancialYear.Name;
                        model.FinancialYearId = investment.FinancialYearId;
                        model.IsDraft = investment.IsDraft;
                        var financialYear = setFinancialYear(investment.FinancialYear);

                        startDate = financialYear.Item2;
                        endDate = financialYear.Item3;

                        if (investment.InvestmentDocuments.Any())
                        {
                            foreach (var doc in investment.InvestmentDocuments)
                            {
                                model.InvestmentDocuments.Add(new InvestmentDocmentDto
                                {
                                    Id = doc.Id,
                                    DocumentName = doc.DocumentName,
                                    DocumentTypeId = doc.InvestmentTypeId ?? 0,
                                    DocumentUrl = doc.DocumentPath
                                });
                            }
                        }
                    }
                }
                else
                {
                    investment = investmentService.GetinvestmentByUserAndFY(CurrentUser.Uid, currentFY.Item1);

                    if (investment != null)
                    {
                        return CustomErrorView($"You already have filled investment details for FY {currentFinYear}, to edit same please use Investment list");
                    }
                }

                while (startDate <= endDate)
                {
                    var month = new InvestmentMonthDto
                    {
                        MonthName = startDate.ToFormatDateString("MMMM-yyyy"),
                        InvMonth = startDate.Month,
                        InvYear = startDate.Year
                    };
                    startDate = startDate.AddMonths(1);

                    if (investment != null && investment.InvestmentMonths.Any())
                    {
                        month.MonthlyRent = investment.InvestmentMonths.FirstOrDefault(x => x.InvMonth == month.InvMonth && x.InvYear == month.InvYear)?.MonthlyRent;
                    }

                    model.InvestmentMonths.Add(month);
                }

                var investmentTypes = investmentService.GetInvestmentTypes(investment?.FinancialYearId ?? currentFY.Item1);

                foreach (var type in investmentTypes.Where(x => x.ClaimType == (byte)Enums.InvestmentClaimType.Exemption))
                {
                    model.DocumentTypeList.Add(new SelectListItem
                    {
                        Value = type.Id.ToString(),
                        Text = type.Name
                    });

                    var typeAmountMap = new InvestmentTypeAmountMapDto
                    {
                        InvestmentTypeId = type.Id,
                        Name = type.Name,
                        ShortNote = type.ShortNote,
                        ClaimType = type.ClaimType
                    };

                    if (investment != null && investment.InvestmentTypeAmountMaps.Any())
                    {
                        typeAmountMap.Amount = investment.InvestmentTypeAmountMaps.FirstOrDefault(x => x.InvestmentTypeId == typeAmountMap.InvestmentTypeId)?.Amount;
                    }

                    model.InvestmentTypeAmountMaps.Add(typeAmountMap);
                }

                foreach (var type in investmentTypes.Where(x => x.ClaimType == (byte)Enums.InvestmentClaimType.Income))
                {
                    model.DocumentTypeList.Add(new SelectListItem
                    {
                        Value = type.Id.ToString(),
                        Text = type.Name
                    });

                    var typeAmountMap = new InvestmentTypeAmountMapDto
                    {
                        InvestmentTypeId = type.Id,
                        Name = type.Name,
                        ShortNote = type.ShortNote,
                        ClaimType = type.ClaimType
                    };

                    if (investment != null && investment.InvestmentTypeAmountMaps.Any())
                    {
                        typeAmountMap.Amount = investment.InvestmentTypeAmountMaps.FirstOrDefault(x => x.InvestmentTypeId == typeAmountMap.InvestmentTypeId)?.Amount;
                    }

                    model.IncomeTypeAmountMaps.Add(typeAmountMap);
                }

                // Add Static Type for "Monthly Rent"
                model.DocumentTypeList.Insert(0, new SelectListItem
                {
                    Value = "0",
                    Text = "Monthly Rent Slip"
                });

                if (investment == null)
                {
                    var userInfo = userLoginService.GetUserInfoByID(CurrentUser.Uid);
                    model.Name = userInfo.Name;
                    model.PAN = userInfo.PanNumber;
                    model.HomeAddress = userInfo.Address;
                    model.DOB = userInfo.DOB.ToFormatDateString("dd/MM/yyyy");
                    model.FinancialYear = string.Format("{0}-{1}", model.InvestmentMonths.First().InvYear, model.InvestmentMonths.Last().InvYear);
                    model.FinancialYearId = currentFY.Item1;
                    model.IsDraft = true;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddEdit(InvestmentDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.CurrentUserId = CurrentUser.Uid;

                    if (model.InvestmentDocuments.Any(x => x.Id == 0))
                    {
                        var docs = model.InvestmentDocuments.Where(x => x.Id == 0 && x.Document != null && x.Document.Length > 0);
                        foreach (var doc in docs)
                        {
                            try
                            {
                                string fileExt = System.IO.Path.GetExtension(doc.Document.FileName.ToLower());
                                string filePath = $"Upload/InvestmentDocument/{model.Name.ToSelfURL()}_{model.FinancialYear}_{doc.DocumentName.ToSelfURL().TrimLength(100, false)}_{DateTime.Now.Ticks}{fileExt}";
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    doc.Document.CopyTo(stream);
                                }

                                //  doc.Document.SaveAs(Server.MapPath("~/" + filePath));

                                doc.DocumentUrl = filePath;

                            }
                            catch { }
                        }
                    }

                    var result = investmentService.Save(model);

                    if (result != null && result.Id > 0)
                    {
                        if (!result.IsDraft)
                        {
                            SendInvestmentEmail(model);

                            return NewtonSoftJsonResult(new RequestOutcome<string>
                            {
                                IsSuccess = true,
                                RedirectUrl = Url.Action("Index")
                            });
                        }
                        else
                        {
                            ShowSuccessMessage("Success", "Investment data saved successfully", false);
                            return NewtonSoftJsonResult(new RequestOutcome<string>
                            {
                                IsSuccess = true,
                                RedirectUrl = Url.Action("AddEdit", new { id = result.Id.ToUrlBase64String() })
                            });
                        }
                    }
                    else
                    {
                        return MessagePartialView("Unable to save investment data");
                    }
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
        public ActionResult DownloadPdf(string id)
        {
            int? invId = !string.IsNullOrWhiteSpace(id) ? id.UrlBaseToInt32() : (int?)null;

            if (invId.HasValue && invId.Value > 0)
            {
                var investment = investmentService.GetinvestmentById(invId.Value);
                if (investment != null && (investment.UserloginId == CurrentUser.Uid || CurrentUser.DeptId == (int)Enums.ProjectDepartment.AccountDepartment))
                {
                    InvestmentDto model = new InvestmentDto();

                    var investmentTypes = investmentService.GetInvestmentTypes(investment.FinancialYearId);

                    model.Id = investment.Id;
                    model.Name = investment.Name;
                    model.FatherName = investment.FatherName;
                    model.AttendanceCode = investment.AttendanceCode;
                    model.PAN = investment.PAN;
                    model.HomeAddress = investment.HomeAddress;
                    model.DOB = investment.DOB.ToFormatDateString("dd MMM yyyy");
                    model.FinancialYear = investment.FinancialYear.Name;
                    model.FinancialYearId = investment.FinancialYearId;
                    model.IsDraft = investment.IsDraft;
                    model.SubmitionDate = investment.ModifyDate.ToFormatDateString("dd MMM yyyy, hh:mm tt");
                    var financialYear = setFinancialYear(investment.FinancialYear);

                    var startDate = financialYear.Item2;
                    var endDate = financialYear.Item3;

                    while (startDate <= endDate)
                    {
                        var month = new InvestmentMonthDto
                        {
                            MonthName = startDate.ToFormatDateString("MMMM-yyyy"),
                            InvMonth = startDate.Month,
                            InvYear = startDate.Year
                        };
                        startDate = startDate.AddMonths(1);

                        if (investment != null && investment.InvestmentMonths.Any())
                        {
                            month.MonthlyRent = investment.InvestmentMonths.FirstOrDefault(x => x.InvMonth == month.InvMonth && x.InvYear == month.InvYear)?.MonthlyRent;
                        }

                        model.InvestmentMonths.Add(month);
                    }

                    foreach (var type in investmentTypes.Where(x => x.ClaimType == (byte)Enums.InvestmentClaimType.Exemption))
                    {
                        model.DocumentTypeList.Add(new SelectListItem
                        {
                            Value = type.Id.ToString(),
                            Text = type.Name
                        });

                        var typeAmountMap = new InvestmentTypeAmountMapDto
                        {
                            InvestmentTypeId = type.Id,
                            Name = type.Name,
                            ShortNote = type.ShortNote,
                            ClaimType = type.ClaimType
                        };

                        if (investment != null && investment.InvestmentTypeAmountMaps.Any())
                        {
                            typeAmountMap.Amount = investment.InvestmentTypeAmountMaps.FirstOrDefault(x => x.InvestmentTypeId == typeAmountMap.InvestmentTypeId)?.Amount;
                        }

                        model.InvestmentTypeAmountMaps.Add(typeAmountMap);
                    }

                    foreach (var type in investmentTypes.Where(x => x.ClaimType == (byte)Enums.InvestmentClaimType.Income))
                    {
                        model.DocumentTypeList.Add(new SelectListItem
                        {
                            Value = type.Id.ToString(),
                            Text = type.Name
                        });

                        var typeAmountMap = new InvestmentTypeAmountMapDto
                        {
                            InvestmentTypeId = type.Id,
                            Name = type.Name,
                            ShortNote = type.ShortNote,
                            ClaimType = type.ClaimType
                        };

                        if (investment != null && investment.InvestmentTypeAmountMaps.Any())
                        {
                            typeAmountMap.Amount = investment.InvestmentTypeAmountMaps.FirstOrDefault(x => x.InvestmentTypeId == typeAmountMap.InvestmentTypeId)?.Amount;
                        }

                        model.IncomeTypeAmountMaps.Add(typeAmountMap);
                    }

                    ////return View(model);
                    return new ViewAsPdf(model)
                    {
                        FileName = $"{model.Name}_{model.FinancialYear}.pdf",
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        CustomSwitches = "--dpi 96"

                        //RotativaOptions = new Rotativa.AspNetCore.Options.
                        //{
                        //    CustomSwitches = "--dpi 96",
                        //    PageSize= Rotativa.Core.Options.Size.A4
                        //}
                    };

                }
            }

            return MessagePartialView("Record not found");
        }

        [HttpGet]
        public ActionResult DownloadDocs(string id)
        {
            try
            {
                int? invId = !string.IsNullOrWhiteSpace(id) ? id.UrlBaseToInt32() : (int?)null;
                if (invId.HasValue && invId.Value > 0)
                {
                    var investment = investmentService.GetinvestmentById(invId.Value);
                    if (investment != null && (investment.UserloginId == CurrentUser.Uid || CurrentUser.DeptId == (int)Enums.ProjectDepartment.AccountDepartment))
                    {
                        var documents = new List<string>();
                        foreach (var doc in investment.InvestmentDocuments)
                        {
                            //string filePath = Server.MapPath("~/" + doc.DocumentPath);
                            string filePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath, doc.DocumentPath);
                            if (System.IO.File.Exists(filePath))
                            {
                                documents.Add(filePath);
                            }
                        }

                        if (!documents.Any())
                        {
                            return CustomErrorView("No file found");
                        }

                        var zipStream = ZipHelper.ZipFilesInStream(documents);

                        /* Set custom headers to force browser to download the file instad of trying to open it */
                        return new FileStreamResult(zipStream, "application/x-zip-compressed")
                        {
                            FileDownloadName = $"{investment.Name}_{investment.FinancialYear.Name}.zip"
                        };

                    }
                    else
                    {
                        return CustomErrorView("Record not found");
                    }
                }
                else
                {
                    return CustomErrorView("Invalid investment id");
                }

            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }

        }

        #region Helper Methods

        private async void SendInvestmentEmail(InvestmentDto model)
        {
            try
            {
                FlexiMail objSendMail = new FlexiMail();
                objSendMail.ValueArray = new string[]
                {
                    model.FinancialYear,
                    model.Name,
                    model.FatherName,
                    model.AttendanceCode,
                    model.InvestmentMonths.Where(x=> x.MonthlyRent.HasValue).Sum(x=> x.MonthlyRent.Value).ToString(),
                    model.InvestmentTypeAmountMaps.Where(x=> x.Amount.HasValue).Sum(x=> x.Amount.Value).ToString(),
                    $"{SiteKey.DomainName}investment/downloaddocs/{model.Id.ToUrlBase64String()}"
                };
                string fromemail = CurrentUser.EmailOffice;
                string userName = CurrentUser.Name;
                objSendMail.From = fromemail;
                objSendMail.To = "accounts@dotsquares.com";
                objSendMail.Subject = $"{model.Name} : Investment Details for FY {model.FinancialYear}";
                objSendMail.MailBodyManualSupply = true;
                objSendMail.MailBody = objSendMail.GetHtml("Investment.html");

                /*Tabassum: This needs to be converted for .net core*/

                //var fileBytes = new ViewAsPdf(model).BuildPdf(ControllerContext);
                var _pdf = new ViewAsPdf(model);
                var fileBytes = await _pdf.BuildFile(actionContextAccessor.ActionContext);
                if (fileBytes != null && fileBytes.Length > 0)
                {
                    var attachment = new System.Net.Mail.Attachment(new MemoryStream(fileBytes), $"{model.Name}_{model.FinancialYear}.pdf");
                    objSendMail.MailAttachments = new System.Net.Mail.Attachment[] { attachment };
                }

                objSendMail.Send();
            }
            catch
            {

            }
        }

        private Tuple<int, DateTime, DateTime> getCurrentFinancialYear()
        {
            var currentFY = investmentService.GetCurrentFinancialYear();
            return setFinancialYear(currentFY);

            //var currentDate = DateTime.Now.Date;
            //var startDate = new DateTime(currentDate.Month >= 4 ? currentDate.Year : currentDate.Year - 1, 4, 1);
            //var endDate = new DateTime(startDate.Year + 1, 3, 31);

            //return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        private Tuple<int, DateTime, DateTime> setFinancialYear(FinancialYear financialYear)
        {

            var startDate = new DateTime(financialYear.FromYear, financialYear.FromMonth, 1);
            // Set Date on 28 or less as minimum days in a month are 28
            // So from DB if month change to Feb or other then no exception
            var endDate = new DateTime(financialYear.ToYear, financialYear.ToMonth, 28);

            return new Tuple<int, DateTime, DateTime>(financialYear.Id, startDate, endDate);
        }

        #endregion
    }
}
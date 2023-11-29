using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Website.Code.LIBS;
using EMS.Web.Modals;
using System.Linq.Expressions;

namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class ExpensesController : BaseController
    {
        #region Fields and Constructor

        private readonly IExpenseService expenseService;
        private readonly IUserLoginService userLoginService;
        private readonly ICurrencyService currencyService;

        public ExpensesController(ICurrencyService _currencyService, IExpenseService _expenseService, IUserLoginService _userLoginService)
        {
            expenseService = _expenseService;
            userLoginService = _userLoginService;
            currencyService = _currencyService;
        }

        private bool IsApprover
        {
            get
            {
                return CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM;
            }
        }

        #endregion

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            var model = new ExpenseIndexDto();

            if (IsApprover)
            {
                int pmId = (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO) ? CurrentUser.Uid : CurrentUser.PMUid;
                model.UserList = userLoginService.GetUsersByPM(pmId)
                                    .Select(x => new SelectListItem { Text = x.Name, Value = x.Uid.ToString() })
                                    .ToList();
                model.IsApprover = true;
            }

            model.StatusList = WebExtensions.GetSelectList<Enums.ExpensePaymentStatus>();
            return View(model);
        }

        [HttpPost]
        [CustomActionAuthorization]
        public IActionResult Index(IDataTablesRequest request, ExpensesFilter expenseFilter)
        {
            var pagingServices = new PagingService<Expense>(request.Start, request.Length);
            var filterExpr = GetExpenseFilterExpression(expenseFilter);
            pagingServices.Filter = filterExpr;

            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "ExpenseDate":
                            return o.OrderByColumn(item, c => c.ExpenseDate);
                        case "CreateDate":
                            return o.OrderByColumn(item, c => c.CreateDate);
                        case "Status":
                            return o.OrderByColumn(item, c => c.Status);
                    }
                }

                return o.OrderByDescending(c => c.CreateDate);
            };

            TempData.Put("ExpensesPagingFilter", expenseFilter);

            int totalCount = 0;
            var response = expenseService.GetExpensesByPaging(out totalCount, pagingServices);

            var summary = pagingServices.Start == 1 ? Newtonsoft.Json.JsonConvert.SerializeObject(expenseService.GetExpensesSummary(pagingServices.Filter)) : null;

            return DataTablesJsonResult(totalCount, request, response.Select((expense, index) => new
            {
                rowIndex = (index + 1) + (request.Start),
                Id = expense.Id,
                Description = expense.Descp,
                ExpenseDate = expense.ExpenseDate.ToFormatDateString("dd MMM yyyy"),
                CreatedBy = expense.UserLogin1.Name,
                CreateDate = expense.CreateDate.ToFormatDateString("dd MMM yyyy hh:mm tt"),
                Status = ((Enums.ExpensePaymentStatus)expense.Status).ToString(),
                ApprovedBy = expense.ApprovedByUid.HasValue ? expense.UserLogin?.Name : "",
                PaidThrough = ((Enums.ExpensePaymentThrough)expense.PaidThrough).GetDescription(),
                Receipt = expense.ReceiptPath,
                ReimburseDate = expense.ReimburseDate.ToFormatDateString("dd MMM yyyy"),
                IsReimbursed = expense.ReimburseDate.HasValue,
                IsReimbursable = expense.Status == (byte)Enums.ExpensePaymentStatus.Approved,
                ReimburseAllowed = CurrentUser.RoleId == (int)Enums.UserRoles.UKPM && expense.CreateByUid != CurrentUser.Uid && expense.Status == (byte)Enums.ExpensePaymentStatus.Approved ? true : IsApprover && CurrentUser.RoleId != (int)Enums.UserRoles.UKPM && expense.Status == (byte)Enums.ExpensePaymentStatus.Approved,
                ApprovalAllowed = CurrentUser.RoleId == (int)Enums.UserRoles.UKPM && expense.CreateByUid != CurrentUser.Uid && expense.Status == (byte)Enums.ExpensePaymentStatus.Pending ? true : IsApprover && CurrentUser.RoleId != (int)Enums.UserRoles.UKPM && expense.Status == (byte)Enums.ExpensePaymentStatus.Pending,
                EditAllowed = expense.CreateByUid == CurrentUser.Uid && expense.Status == (byte)Enums.ExpensePaymentStatus.Pending,
                deleteAllowed = IsApprover,
                Amount = $"{expense.Currency.CurrSign} {string.Format("{0:0.00}", expense.Amount)}"
            }), new Dictionary<string, object> { { "Summary", summary } });
        }

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult AddEdit(int? id)
        {
            try
            {
                var model = new ExpenseDto();

                var Currency = currencyService.GetCurrency();

                model.CurrencyList = Currency.Select(x => new SelectListItem()
                {
                    Text = x.CurrSign,
                    Value = x.Id.ToString()
                }).ToList();

                model.CurrencyId = Currency.FirstOrDefault(x => x.CurrName == "GBP")?.Id ?? 0;

                if (id.HasValue && id.Value > 0)
                {
                    var expenseEntity = expenseService.GetExpenseById(id.Value);

                    if (expenseEntity != null)
                    {
                        if (expenseEntity.CreateByUid == CurrentUser.Uid && expenseEntity.Status == (byte)Enums.ExpensePaymentStatus.Pending)
                        {
                            model.Id = expenseEntity.Id;
                            model.Descp = expenseEntity.Descp;
                            model.ExpenseDate = expenseEntity.ExpenseDate.ToFormatDateString("dd/MM/yyyy");
                            model.Amount = expenseEntity.Amount.ToString();
                            model.CurrencyId = expenseEntity.CurrencyId;
                            model.PaidThrough = expenseEntity.PaidThrough;
                            model.ReceiptPath = expenseEntity.ReceiptPath;
                        }
                        else
                        {
                            return MessagePartialView("Invalid access or status has been updated");
                        }
                    }
                    else
                    {
                        return MessagePartialView("Unable to find record");
                    }
                }

                model.ExpensePaymentThroughList = WebExtensions.GetSelectList<Enums.ExpensePaymentThrough>();

                return PartialView("_AddEdit", model);
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult AddEdit(ExpenseDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string folderPath = "Upload/ExpenseReceipt";
                    model.ReceiptPath = GeneralMethods.SaveFile(model.Receipt, folderPath, "");
                    model.ReceiptPath = model.ReceiptPath.HasValue() ? $"{folderPath}/{model.ReceiptPath}" : null;
                    model.CurrentUserId = CurrentUser.Uid;

                    var result = expenseService.Save(model);

                    if (result != null && result.Id > 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = "Expense saved successfully"
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
            else
            {
                return CreateModelStateErrors();
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult UpdateStatus(ExpenseApproveDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (IsApprover)
                    {
                        model.CurrentUserId = CurrentUser.Uid;
                        model.Status = model.IsApproved ? (byte)Enums.ExpensePaymentStatus.Approved : (byte)Enums.ExpensePaymentStatus.Rejected;
                        expenseService.UpdateStatus(model);

                        return NewtonSoftJsonResult(new RequestOutcome<string>
                        {
                            IsSuccess = true,
                            Message = $"Selected expenses marked as {(Enums.ExpensePaymentStatus)model.Status} successfully"
                        });
                    }
                    else
                    {
                        return MessagePartialView("Unauthorized Access");
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
        public ActionResult DeleteExpenses(int id)
        {
            try
            {
                ExpenseDto model = new ExpenseDto();
                model.Id = id;
                return PartialView("_DeleteExpenses", model);
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.InnerException?.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult DeleteExpenses(ExpenseDto model)
        {

            try
            {
                if (IsApprover)
                {

                    expenseService.DeleteExpenses(model.Id);

                    return NewtonSoftJsonResult(new RequestOutcome<string>
                    {
                        IsSuccess = true,
                        Message = string.Format("Expense has been deleted successfully")
                    });
                }
                else
                {
                    return MessagePartialView("Unauthorized Access");
                }
            }
            catch (Exception ex)
            {
                return MessagePartialView(ex.Message);
            }

        }


        //[HttpGet]
        //[CustomActionAuthorization]
        //public ActionResult MarkAsReimbursed(int id)
        //{
        //    try
        //    {
        //        if (id > 0)
        //        {
        //            var model = new ExpenseDto { Id = id };
        //            var expenseEntity = expenseService.GetExpenseById(id);

        //            if (expenseEntity != null)
        //            {
        //                if (IsApprover && expenseEntity.Status == (byte)Enums.ExpensePaymentStatus.Approved && !expenseEntity.ReimburseDate.HasValue)
        //                {
        //                    model.Descp = expenseEntity.Descp;
        //                    model.ExpenseDate = expenseEntity.ExpenseDate.ToFormatDateString("dd MMM yyyy");
        //                    model.Amount = $"{expenseEntity.Currency.CurrSign} {string.Format("{0:0.00}", expenseEntity.Amount)}";
        //                    model.CreatedByUser = expenseEntity.UserLogin1.Name;

        //                    return PartialView("_MarkAsReimbursed", model);
        //                }
        //                else
        //                {
        //                    return MessagePartialView("Invalid access or status already has been updated");
        //                }
        //            }
        //            else
        //            {
        //                return MessagePartialView("Unable to find record");
        //            }
        //        }
        //        else
        //        {
        //            return MessagePartialView("Invalid parameters");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return CustomErrorView(ex.Message);
        //    }
        //}

        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult MarkAsReimbursed()
        {
            try
            {
                return PartialView("_MarkAsReimbursed", new ExpenseReimburserdDto());

                //if (id > 0)
                //{
                //    var model = new ExpenseDto { Id = id };
                //    var expenseEntity = expenseService.GetExpenseById(id);

                //    if (expenseEntity != null)
                //    {
                //        if (IsApprover && expenseEntity.Status == (byte)Enums.ExpensePaymentStatus.Approved && !expenseEntity.ReimburseDate.HasValue)
                //        {
                //            model.Descp = expenseEntity.Descp;
                //            model.ExpenseDate = expenseEntity.ExpenseDate.ToFormatDateString("dd MMM yyyy");
                //            model.Amount = $"{expenseEntity.Currency.CurrSign} {string.Format("{0:0.00}", expenseEntity.Amount)}";
                //            model.CreatedByUser = expenseEntity.UserLogin1.Name;

                //            return PartialView("_MarkAsReimbursed", model);
                //        }
                //        else
                //        {
                //            return MessagePartialView("Invalid access or status already has been updated");
                //        }
                //    }
                //    else
                //    {
                //        return MessagePartialView("Unable to find record");
                //    }
                //}
                //else
                //{
                //    return MessagePartialView("Invalid parameters");
                //}
            }
            catch (Exception ex)
            {
                return CustomErrorView(ex.Message);
            }
        }

        //[HttpPost]
        //[CustomActionAuthorization]
        //public ActionResult MarkAsReimbursed(ExpenseDto model)
        //{
        //    if (model.Id > 0 && model.ReimburseDate.HasValue())
        //    {
        //        try
        //        {
        //            if (IsApprover)
        //            {
        //                model.CurrentUserId = CurrentUser.Uid;

        //                expenseService.MarkAsReimbursed(model);

        //                return NewtonSoftJsonResult(new RequestOutcome<string>
        //                {
        //                    IsSuccess = true,
        //                    Message = $"Selected record marked as Paid successfully"
        //                });
        //            }
        //            else
        //            {
        //                return MessagePartialView("Unauthorized Access");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return MessagePartialView(ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return MessagePartialView("Invalid parameters");
        //    }
        //}

        [HttpPost]
        [CustomActionAuthorization]
        public ActionResult MarkAsReimbursed(ExpenseReimburserdDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (IsApprover)
                    {
                        model.CurrentUserId = CurrentUser.Uid;

                        if (model.ExpenseIds != null && model.ExpenseIds.Any())
                        {
                            expenseService.MarkAsReimbursed(model);

                            return NewtonSoftJsonResult(new RequestOutcome<string>
                            {
                                IsSuccess = true,
                                Message = $"Selected records marked as Paid successfully"
                            });
                        }
                        return MessagePartialView("No record is find to update records");
                    }
                    else
                    {
                        return MessagePartialView("Unauthorized Access");
                    }
                }
                catch (Exception ex)
                {
                    return MessagePartialView(ex.Message);
                }
            }
            else
            {
                return MessagePartialView("Invalid parameters");
            }
        }

        [HttpGet]
        public ActionResult DownloadReport()
        {
            var expenseFilters = TempData.Get<ExpensesFilter>("ExpensesPagingFilter");
            if (expenseFilters != null)
            {
                var pagingFitlers = new PagingService<Expense>(1, int.MaxValue);
                pagingFitlers.Filter = GetExpenseFilterExpression(expenseFilters);
                int totalCount = 0;
                var response = expenseService.GetExpensesByPaging(out totalCount, pagingFitlers);
                var summary = expenseService.GetExpensesSummary(pagingFitlers.Filter);

                var expensesReport = response.Select(x => new
                {
                    ExpenseDate = x.ExpenseDate.ToFormatDateString("dd MMM yyyy"),
                    Description = x.Descp,
                    //Amount = $"{x.Currency.CurrSign} {string.Format("{0:0.00}", x.Amount)}",
                    Amount = x.Amount,
                    CompanyCard = x.PaidThrough == (byte)Enums.ExpensePaymentThrough.CompanyCard ? $"{x.Currency.CurrSign} {string.Format("{0:0.00}", x.Amount)}" : "",
                    CashOrPersonalCard = x.PaidThrough == (byte)Enums.ExpensePaymentThrough.CashOrPersonalCard ? $"{x.Currency.CurrSign} {string.Format("{0:0.00}", x.Amount)}" : "",
                    AddedBy = x.UserLogin1.Name,
                    AddedDate = x.CreateDate.ToFormatDateString("dd MMM yyyy hh:mm tt"),
                    Status = $"{(Enums.ExpensePaymentStatus)x.Status}",
                    ApprovedBy = x.ApprovedByUid.HasValue ? x.UserLogin?.Name : "",
                    Paid = x.ReimburseDate.HasValue ? $"Paid on {x.ReimburseDate.ToFormatDateString("dd MMM yyyy")}" : "Not Paid"
                }).ToList();

                var summaryReport = summary.Select(x => new
                {
                    Status = x.Status,
                    CompanyCard = string.Join(", ", x.CompanyCardSummary),
                    CashOrPersonalCard = string.Join(", ", x.CashOrPersonalCardSummary),
                    TotalSummary = string.Join(", ", x.TotalSummary)
                }).ToList();

                List<ExportExcelData> dataToExport = new List<ExportExcelData>();

                dataToExport.Add(new ExportExcelData
                {
                    Heading = "Expenses Summary",
                    ShowSrNo = true,
                    ColumnsToTake = new string[] { "Status", "CompanyCard", "CashOrPersonalCard", "TotalSummary" },
                    DataTable = ExportExcelHelper.ToDataTable(summaryReport)
                });

                dataToExport.Add(new ExportExcelData
                {
                    Heading = "Expenses Report",
                    ShowSrNo = true,
                    ColumnsToTake = new string[] { "ExpenseDate", "Description", "Amount", "CompanyCard", "CashOrPersonalCard", "AddedBy", "AddedDate", "Status", "ApprovedBy", "Paid" },
                    DataTable = ExportExcelHelper.ToDataTable(expensesReport)
                });

                byte[] filecontent = ExportExcelHelper.ExportExcel(dataToExport);
                string fileName = $"ExpensesReport_{DateTime.Now.Ticks}.xlsx";
                return File(filecontent, ExportExcelHelper.ExcelContentType, fileName);
            }

            return Content("Unable to get filters");
        }

        private Expression<Func<Expense, bool>> GetExpenseFilterExpression(ExpensesFilter expenseFilter)
        {
            var expr = PredicateBuilder.True<Expense>();
            var currentUserId = CurrentUser.Uid;
            var IsAdditionalAccess = userLoginService.CheckAdditionalAccess(CurrentUser.Uid);            

            if (CurrentUser.RoleId == (int)Enums.UserRoles.PM || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.PMO)
            {
                if (CurrentUser.RoleId == (int)Enums.UserRoles.UKPM && IsAdditionalAccess)
                {
                    expr = expr.And(x => x.CreateByUid == currentUserId || x.UserLogin1.PMUid == CurrentUser.PMUid);
                }
                else
                {
                    expr = expr.And(x => x.CreateByUid == currentUserId || x.UserLogin1.PMUid == currentUserId || x.UserLogin1.TLId == currentUserId);
                }
            }
            else if (CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                expr = expr.And(x => x.CreateByUid == currentUserId || x.UserLogin1.PMUid == CurrentUser.PMUid);
            }
            else
            {
                expr = expr.And(x => x.CreateByUid == currentUserId);
            }

            if (expenseFilter.UserId.HasValue && expenseFilter.UserId.Value > 0)
            {
                expr = expr.And(x => x.CreateByUid == expenseFilter.UserId.Value);
            }

            if (expenseFilter.Status.HasValue && Enum.IsDefined(typeof(Enums.ExpensePaymentStatus), expenseFilter.Status.Value))
            {
                expr = expr.And(x => x.Status == (byte)expenseFilter.Status.Value);
            }

            if (expenseFilter.DateFrom.HasValue())
            {
                var startDate = expenseFilter.DateFrom.ToDateTime("dd/MM/yyyy");
                expr = expr.And(x => x.ExpenseDate == startDate);
            }

            if (expenseFilter.DateTo.HasValue())
            {
                var endDate = expenseFilter.DateTo.ToDateTime("dd/MM/yyyy");
                expr = expr.And(x => x.ExpenseDate == endDate);
            }
            return expr;
        }
    }
}
using EMS.Core;
using EMS.Data;
using EMS.Dto;
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
using System.IO;
using System.Linq;

namespace EMS.Web.Controllers {
    [CustomAuthorization()]
    public class MedicalDataController : BaseController {
        private readonly IEmployeeMedicalService _medicalDataService;
        private readonly IUserLoginService _userService;

        public MedicalDataController(IEmployeeMedicalService medicalDataService, IUserLoginService userService) {
            _medicalDataService = medicalDataService;
            _userService = userService;
        }

        [CustomActionAuthorization]
        public ActionResult Index() {
            ViewBag.PM = _userService.GetPMAndPMOHRUsers().Select(p => new SelectListItem { Text = p.Name, Value = p.Uid.ToString() }).ToList();
            return View();
        }

        public IActionResult GetList(DataTables.AspNet.Core.IDataTablesRequest request, MedicalDataSearchFilter searchFilter) {
            var pagingService = new PagingService<EmployeeMedicalData>(request.Start, request.Length);

            var expr = PredicateBuilder.True<EmployeeMedicalData>();

            expr = expr.And(p => p.UserLogin.IsActive == true);
            if (CurrentUser.RoleId != (int)Enums.UserRoles.HRBP) {
                expr = expr.And(p => p.UserLogin.PMUid == CurrentUser.PMUid || p.UserId == CurrentUser.Uid || p.UserLogin.PMUid == CurrentUser.Uid || p.UserLogin.TLId == CurrentUser.Uid);
            }
            if (searchFilter.pmid.HasValue) {
                expr = expr.And(p => p.UserLogin.PMUid == searchFilter.pmid);
            }

            ContextProvider.HttpContext.Session.SetString("relativestatus", searchFilter.status.ToString());
            ContextProvider.HttpContext.Session.SetString("txtSearch", searchFilter.txtEmployee != null ? searchFilter.txtEmployee.ToString() : "");
            ContextProvider.HttpContext.Session.SetString("dateFrom", searchFilter.dateFrom != null ? searchFilter.dateFrom.ToString() : "");
            ContextProvider.HttpContext.Session.SetString("dateTo", searchFilter.dateTo != null ? searchFilter.dateTo.ToString() : "");

            //Session["relativestatus"] = status.ToString();

            if (searchFilter.status != "All") {
                if (searchFilter.status == "1") {
                    expr = expr.And(p => p.ShowRelative == true);
                }
                else {
                    expr = expr.And(p => p.ShowRelative == false || p.ShowRelative == null);
                }
            }

           

            if (searchFilter.txtEmployee != null && searchFilter.txtEmployee != string.Empty) {
                expr = expr.And(p => p.EmployeeCode.Trim().ToLower().Contains(searchFilter.txtEmployee.Trim().ToLower()) || p.Name.Trim().ToLower().Contains(searchFilter.txtEmployee.Trim().ToLower()));
            }


            DateTime? startDate = searchFilter.dateFrom.ToDateTime("dd/MM/yyyy");
            DateTime? endDate = searchFilter.dateTo.ToDateTime("dd/MM/yyyy");


            if (startDate.HasValue && endDate.HasValue) {
                expr = expr.And(L => L.UserLogin.JoinedDate >= startDate.Value && L.UserLogin.JoinedDate <= endDate.Value);

            }
            else if (startDate.HasValue) {

                expr = expr.And(L => L.UserLogin.JoinedDate >= startDate.Value);

            }
            else if (endDate.HasValue) {
                expr = expr.And(L => L.UserLogin.JoinedDate <= endDate.Value);
            }

            pagingService.Filter = expr;

            pagingService.Sort = (o) => {
                foreach (var item in request.SortedColumns()) {
                    switch (item.Name) {
                        case "Name":
                            return o.OrderByColumn(item, c => c.Name);
                        case "Designation":
                            return o.OrderByColumn(item, c => c.Designation);
                        case "EmployeeCode":
                            return o.OrderByColumn(item, c => c.EmployeeCode);
                        case "AddedDate":
                            return o.OrderByColumn(item, c => c.AddedDate);
                        case "RelativeData":
                            return o.OrderByColumn(item, c => c.ShowRelative);
                    }
                }

                return o.OrderByDescending(c => c.AddedDate);
            };

            int totalCount = 0;
            var response = _medicalDataService.GetEmployeeMedicals(out totalCount, pagingService);
            return DataTablesJsonResult(totalCount, request, response.Select((x, index) => new {
                rowId = request.Start + index + 1,
                UserId = x.UserId,
                Name = x.Name,
                EmployeeCode = !string.IsNullOrEmpty(x.EmployeeCode) ? x.EmployeeCode : "",
                RelativeData = x.ShowRelative == true ? "Yes" : "No",
                //RelativeData = x.EmployeeRelativeMedicalDatas.Count>0 ? "Yes" : "No",
                JoiningDate = x.UserLogin.JoinedDate.ToFormatDateString("ddd, MMM dd, yyyy"),
                DOB = x.Dob.ToFormatDateString("ddd, MMM dd, yyyy"),
                Designation = x.Designation,
                AddedDate = x.AddedDate.ToString("ddd, MMM dd, yyyy")
            }));

        }

        [CustomActionAuthorization]
        [HttpGet]
        public ActionResult ManageMedicalData() {
            var data = new MedicalDataDto { IsActive = true, AddedDate = DateTime.Now, UserId = CurrentUser.Uid };
            if (CurrentUser != null) {
                var model = _medicalDataService.GetEmployeeMedicalDataByUserid(CurrentUser.Uid);
                if (model != null) {
                    data.Id = model.Id;
                    data.AddedDate = model.AddedDate;
                    data.ShowRelative = model.ShowRelative ?? false;
                    data.Designation = model.Designation;
                    data.DOB = model.Dob.ToString("dd/MM/yyyy");
                    data.EmployeeCode = model.EmployeeCode;
                    data.Gender = model.Gender;
                    data.Name = model.Name;
                    data.PremiumTotal = model.PremiumTotal.HasValue ? model.PremiumTotal.Value : 0;
                    data.PremiumPerMonth = model.PremiumPerMonth.HasValue ? model.PremiumPerMonth.Value : 0;
                    data.TotalCoverage = model.TotalCoverage.HasValue ? model.TotalCoverage.Value : 0;
                    data.Validity = model.Validity.HasValue ? model.Validity.Value : 0;
                    data.NameTitle = model.Title;
                    data.Relatives = model.EmployeeRelativeMedicalDatas.Select(c => new RelativeMedicalDataDto { AddedDate = c.AddedDate, DOB = c.Dob.ToString("dd/MM/yyyy"), IsActive = c.IsActive, Gender = c.Gender, Name = c.Name, Relation = c.Relation, NameTitle = c.Title, Id = c.Id }).ToList();
                }
                else {
                    var user = _userService.GetUserInfoByID(CurrentUser.Uid);
                    if (user != null) {
                        data.Designation = user.JobTitle;
                        data.DOB = user.DOB.HasValue ? user.DOB.Value.ToString("dd/MM/yyyy") : string.Empty;
                        data.EmployeeCode = user.EmpCode;
                        data.ShowRelative = false;
                        if (!string.IsNullOrEmpty(user.Gender)) {
                            data.Gender = (byte)(user.Gender.ToLower().Trim() == "f" ? Enums.Gender.Female : Enums.Gender.Male);
                            data.NameTitle = (byte)(user.Gender.ToLower().Trim() == "f" ? Enums.Title.Miss : Enums.Title.Mr);
                        }
                        data.Name = CurrentUser.Name;
                    }
                }
            }
            return View(data);
        }

        [HttpPost]
        public ActionResult ManageMedicalData(MedicalDataDto model) {

            var data = new EmployeeMedicalData();
            data.AddedDate = DateTime.Now;

            if (model.Id != 0) {
                data = _medicalDataService.GetEmployeeMedicalData(model.Id);
            }
            data.IsActive = true;
            data.Designation = model.Designation;
            data.Dob = model.DOB.ToDateTime().Value;
            data.EmployeeCode = model.EmployeeCode != null ? model.EmployeeCode : string.Empty;
            data.Gender = model.Gender;
            data.Name = model.Name;
            data.Title = model.NameTitle;
            data.UserId = model.UserId;
            data.ShowRelative = model.ShowRelative;
            model.Relatives.ForEach(c => {
                try {
                    if (!string.IsNullOrEmpty(c.Name) && !string.IsNullOrEmpty(c.DOB)) {
                        if (c.Id == 0) {
                            data.EmployeeRelativeMedicalDatas.Add(new EmployeeRelativeMedicalData { Id = c.Id, AddedDate = DateTime.Now, Dob = c.DOB.ToDateTime().Value, IsActive = true, Gender = c.Gender, Name = c.Name, Relation = c.Relation, Title = c.NameTitle });
                        }
                        else {
                            var item = data.EmployeeRelativeMedicalDatas.FirstOrDefault(x => x.Id == c.Id);
                            if (item != null) {
                                item.Id = c.Id; item.AddedDate = DateTime.Now;
                                item.Dob = c.DOB.ToDateTime().Value;
                                item.IsActive = true;
                                item.Gender = c.Gender;
                                item.Name = c.Name;
                                item.Relation = c.Relation;
                                item.Title = c.NameTitle;
                            }
                        }
                    }
                }
                catch { }
            }
            );
            _medicalDataService.Save(data, model.Id == 0);
            ShowSuccessMessage("Success", "Record saved succesfully", false);
            return RedirectToAction("ManageMedicalData");
        }

        [CustomActionAuthorization]
        public ActionResult View(int id) {
            var model = _medicalDataService.GetEmployeeMedicalDataByUserid(id);

            if (model != null) {
                var data = new MedicalDataDto();
                data.Id = model.Id;
                data.AddedDate = model.AddedDate;
                data.IsActive = model.IsActive;
                data.Designation = model.Designation;
                data.DOB = model.Dob.ToString("MMM dd,yyyy");
                data.EmployeeCode = model.EmployeeCode;
                data.Gender = model.Gender;
                data.Name = model.Name;
                data.NameTitle = model.Title;
                data.Relatives = model.EmployeeRelativeMedicalDatas.Select(c => new RelativeMedicalDataDto { AddedDate = c.AddedDate, DOB = c.Dob.ToString("MMM dd,yyyy"), IsActive = c.IsActive, Gender = c.Gender, Name = c.Name, Relation = c.Relation, NameTitle = c.Title, Id = c.Id }).ToList();
                return PartialView("_displaymedicaldata", data);
            }
            return PartialView("_displaymedicaldata");
        }

        #region Medical Data Report
        /// <summary>
        /// Export medical data report in excel format
        /// </summary>
        /// <returns>medical data report in excel format</returns>
        /// 
        [HttpGet]
        public ActionResult ExportMedicalDataReportToExcel(int? pmid) {
            string status = HttpContext.Session.GetString("relativestatus");
            string empSearch = HttpContext.Session.GetString("txtSearch");
            string joiningDatefrom = HttpContext.Session.GetString("dateFrom");
            string joiningDateTo = HttpContext.Session.GetString("dateTo");

            DateTime? startDate = joiningDatefrom.ToDateTime("dd/MM/yyyy");
            DateTime? endDate = joiningDateTo.ToDateTime("dd/MM/yyyy");

            bool statusType = false;
            if (status != "All") {
                if (status == "1") {
                    statusType = true;
                }
                else {
                    statusType = false;
                }
            }
            List<EmployeeMedicalData> employeeMedicalData = _medicalDataService.GetEmployeeMedicals().ToList();
            if (CurrentUser.RoleId != (int)Enums.UserRoles.HRBP) {
                employeeMedicalData = employeeMedicalData.Where(p => p.UserLogin.PMUid == CurrentUser.PMUid || p.UserId == CurrentUser.Uid || p.UserLogin.PMUid == CurrentUser.Uid || p.UserLogin.TLId == CurrentUser.Uid).ToList();
            }
            if (pmid.HasValue) {
                employeeMedicalData = employeeMedicalData.Where(p => p.UserLogin.PMUid == pmid && p.ShowRelative == statusType).ToList();
            }
            if (empSearch != null && empSearch != string.Empty) {
                employeeMedicalData = employeeMedicalData.Where(p => p.EmployeeCode.Trim().ToLower().Contains(empSearch.Trim().ToLower()) || p.Name.Trim().ToLower().Contains(empSearch.Trim().ToLower())).ToList();
            }

            if (startDate.HasValue && endDate.HasValue) {
                employeeMedicalData = employeeMedicalData.Where(L => L.UserLogin.JoinedDate >= startDate.Value && L.UserLogin.JoinedDate <= endDate.Value).ToList();
            }
            else if (startDate.HasValue) {
                employeeMedicalData = employeeMedicalData.Where(L => L.UserLogin.JoinedDate >= startDate.Value).ToList();
            }
            else if (endDate.HasValue) {
                employeeMedicalData = employeeMedicalData.Where(L => L.UserLogin.JoinedDate <= endDate.Value).ToList();
            }

            employeeMedicalData = status == "All" ? employeeMedicalData : status=="1" ? employeeMedicalData.Where(p => p.ShowRelative == statusType).ToList(): employeeMedicalData.Where(p => p.ShowRelative == statusType || p.ShowRelative ==null).ToList();


            List<MedicalDataDto> responseReport = employeeMedicalData.OrderByDescending(o => o.AddedDate).Select(s => new MedicalDataDto() {
                Name = s.Name,
                EmployeeCode = s.EmployeeCode != null ? s.EmployeeCode : string.Empty,
                DOB = s.Dob != null ? s.Dob.ToFormatDateString("dd-MM-yyyy") : string.Empty,
                JoiningDate = s.UserLogin.JoinedDate != null ? s.UserLogin.JoinedDate.ToFormatDateString("dd-MM-yyyy") : string.Empty,
                EmpGender = Enum.GetName(typeof(Enums.Gender), s.Gender),
                Designation = s.Designation != null ? s.Designation : string.Empty,
                Relatives = s.EmployeeRelativeMedicalDatas.Select(x => new RelativeMedicalDataDto() {
                    Name = x.Name,
                    RelationName = Enum.GetName(typeof(Enums.RelationType), x.Relation),
                    DOB = x.Dob != null ? x.Dob.ToString("dd-MM-yyyy") : string.Empty,
                    RelativeGender = Enum.GetName(typeof(Enums.Gender), x.Gender)
                }).ToList()
            }).ToList();

            string Reportname = "Medical_data_report";
            int subsheet = 0;
            List<ExportExcelColumn> excelColumn = new List<ExportExcelColumn>();

            excelColumn.Add(new ExportExcelColumn { ColumnName = "Sr. No.", PropertyName = "SrNo", ColumnWidth = 1000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Name", PropertyName = "Name", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "EMP Code", PropertyName = "EmployeeCode", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Joining Date", PropertyName = "JoiningDate", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "DOB", PropertyName = "DOB", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Gender", PropertyName = "EmpGender", ColumnWidth = 3500 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Designation", PropertyName = "Designation", ColumnWidth = 10000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Relative Name", PropertyName = "RelativeName", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Relative Relation", PropertyName = "RelativeRelation", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Relative DOB", PropertyName = "RelativeDOB", ColumnWidth = 5000 });
            excelColumn.Add(new ExportExcelColumn { ColumnName = "Relative Gender", PropertyName = "RelativeGender", ColumnWidth = 5000 });

            var memoryStream = ToExportToExcel(responseReport, subsheet, Reportname, excelColumn);

            return File(memoryStream.ToArray(), "application/vnd.ms-excel", Reportname.Trim().Replace(" ", "_") + "_" + DateTime.Now.ToString("hh_mm_ss_tt") + ".xls");
        }
        public static MemoryStream ToExportToExcel(List<MedicalDataDto> lsObj, int isSubsheet, string Reportname, List<ExportExcelColumn> excelColumn) {
            MemoryStream response = new MemoryStream();
            if (lsObj != null && lsObj.Count() > 0) {
                bool columnFlag = false;
                List<string> props = new List<string>();
                List<string> childprops = new List<string>();
                //Get the column names of Employee Medical Data
                if (excelColumn != null && excelColumn.Count > 0) {
                    columnFlag = true;
                    props = excelColumn.Select(s => s.PropertyName.Trim()).ToList();
                }

                //Get the column names of Employee Relative Medical Data        
                if (props != null && props.Count > 0) {
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
                    foreach (var item in props) {
                        headerRow.CreateCell(count, CellType.STRING).SetCellValue(
                         columnFlag ? excelColumn.Where(x => x.PropertyName == item).Select(x => x.ColumnName).FirstOrDefault() : item);
                        count = count + 1;
                    }

                    for (int i = 0; i < headerRow.LastCellNum; i++) {
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

                    for (int i = 0; i < lsObj.Count(); i++) {
                        var row = sheet.CreateRow(rowNumber);
                        count = 0;
                        row.CreateCell(count++).SetCellValue(i + 1); // Sr. No.
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Name));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].EmployeeCode));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].JoiningDate));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].DOB));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].EmpGender));
                        row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Designation));

                        rowNumber++;
                        Row childRow = null;
                        for (int j = 0; j < lsObj[i].Relatives.Count; j++) {
                            //if (j == 0)
                            //{
                            //    row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Relatives[j].Name));
                            //    row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Relatives[j].RelationName));
                            //    row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Relatives[j].DOB));
                            //    row.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Relatives[j].RelativeGender));
                            //    count = 5; //5 index number
                            //}
                            //else if (j < lsObj[i].Relatives.Count)
                            //{
                            childRow = sheet.CreateRow(rowNumber++);
                            childRow.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Relatives[j].Name));
                            childRow.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Relatives[j].RelationName));
                            childRow.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Relatives[j].DOB));
                            childRow.CreateCell(count++).SetCellValue(Convert.ToString(lsObj[i].Relatives[j].RelativeGender));
                            count = 7; //7 index number
                            //}
                            for (int k = 0; k < headerRow.LastCellNum; k++) {
                                string columnName = headerRow.GetCell(k).ToString();
                                sheet.SetColumnWidth(k, 5000);
                            }
                            for (int k = 7; k < headerRow.LastCellNum; k++) {
                                if (childRow != null) {
                                    string columnName = childRow.GetCell(k).ToString();
                                    sheet.SetColumnWidth(k, 5000);
                                }
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
        #endregion
    }
}